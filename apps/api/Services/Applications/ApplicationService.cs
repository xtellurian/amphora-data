using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Applications;
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

            if (userReadRes.Entity.Organisation.Account.Plan.PlanType == Common.Models.Organisations.Accounts.Plan.PlanTypes.Institution)
            {
                // check the org is on institution
                model.OrganisationId = userReadRes.Entity.OrganisationId;
                model = await store.CreateAsync(model);
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity, model);
            }
            else
            {
                return new EntityOperationResult<ApplicationModel>(userReadRes.Entity,
                    "Institution Plan is required to create applications");
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
                // check the org is on institution
                model.OrganisationId = userReadRes.Entity.OrganisationId;
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