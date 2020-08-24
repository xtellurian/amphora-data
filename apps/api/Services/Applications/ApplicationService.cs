using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Applications
{
    public class ApplicationService : IApplicationService
    {
        private readonly IEntityStore<ApplicationModel> store;
        private readonly IUserDataService userDataService;
        private readonly ILogger<ApplicationService> logger;

        public ApplicationService(IEntityStore<ApplicationModel> store,
                                  IUserDataService userDataService,
                                  ILogger<ApplicationService> logger)
        {
            this.store = store;
            this.userDataService = userDataService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<ApplicationModel>> CreateAsync(ClaimsPrincipal principal, ApplicationModel model)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Failed)
            {
                return new EntityOperationResult<ApplicationModel>("Unknown user");
            }

            if (IsInvalid(model, out var message))
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, message);
            }

            // check name doesn't already exist
            if (await store.CountAsync(_ => _.Name == model.Name) > 0)
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, $"Application Name {model.Name} already exists.");
            }

            if (userReadRes.Entity.Organisation.Account.Plan.PlanType == Plan.PlanTypes.Institution
            || userReadRes.Entity.Organisation.Account.Plan.PlanType == Plan.PlanTypes.Glaze)
            {
                // check the org is on institution
                model.OrganisationId = userReadRes.Entity.OrganisationId;
                model = await store.CreateAsync(model);
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, model);
            }
            else
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity,
                    "Institution or Glaze plan is required to create applications");
            }
        }

        public async Task<EntityOperationResult<ApplicationModel>> ReadAsync(ClaimsPrincipal principal, string id)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Failed)
            {
                return new EntityOperationResult<ApplicationModel>("Unknown user");
            }

            var model = await store.ReadAsync(id);

            if (model is null || model.OrganisationId != userReadRes.Entity.OrganisationId)
            {
                // not matching, return not found.
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, "Application not found.");
            }
            else
            {
                // found it
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, model);
            }
        }

        public async Task<EntityOperationResult<IEnumerable<ApplicationModel>>> ListAsync(ClaimsPrincipal principal)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Failed)
            {
                return new EntityOperationResult<IEnumerable<ApplicationModel>>("Unknown user");
            }

            var apps = await store.QueryAsync(a => a.OrganisationId == userReadRes.Entity.OrganisationId, 0, 50);
            if (userReadRes.Entity.Organisation.IsAdministrator(userReadRes.User))
            {
                return new EntityOperationResult<IEnumerable<ApplicationModel>>(userReadRes.Entity, apps);
            }
            else
            {
                return new EntityOperationResult<IEnumerable<ApplicationModel>>(userReadRes.Entity, "User is not an administrator. Cannot list apps");
            }
        }

        public async Task<EntityOperationResult<ApplicationModel>> UpdateAsync(ClaimsPrincipal principal, ApplicationModel model)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Failed)
            {
                return new EntityOperationResult<ApplicationModel>("Unknown user");
            }

            if (userReadRes.Entity.OrganisationId != model.OrganisationId)
            {
                // not matching, return not found.
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, $"Application not found in Organisation({userReadRes.Entity.OrganisationId}).");
            }
            else
            {
                // found it, apply the update
                model = await store.UpdateAsync(model);
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, model);
            }
        }

        public async Task<EntityOperationResult<ApplicationModel>> DeleteAsync(ClaimsPrincipal principal, string id)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Failed)
            {
                return new EntityOperationResult<ApplicationModel>("Unknown user");
            }

            var model = await store.ReadAsync(id);
            if (model is null)
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, "Unknown Application Id");
            }

            if (userReadRes.Entity.OrganisationId == model.OrganisationId && userReadRes.Entity.IsAdministrator())
            {
                // clean up the locations first.
                model.Locations.Clear();
                model = await store.UpdateAsync(model);
                // now delete the application
                await store.DeleteAsync(model);
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, true);
            }
            else
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity,
                    $"You must be an administrator on Organisation({model.OrganisationId})");
            }
        }

        private bool IsInvalid(ApplicationModel model, out string message)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                message = "Name must not be empty";
                return true;
            }

            message = "";
            return false;
        }
    }
}