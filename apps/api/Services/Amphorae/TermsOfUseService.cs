using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class TermsOfUseService : ITermsOfUseService
    {
        private readonly IEntityStore<TermsOfUseModel> store;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly ILogger<TermsOfUseService> logger;

        public TermsOfUseService(IEntityStore<TermsOfUseModel> store,
                                 IUserDataService userDataService,
                                 IPermissionService permissionService,
                                 ILogger<TermsOfUseService> logger)
        {
            this.store = store;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<IEnumerable<TermsOfUseModel>>> ListAsync(ClaimsPrincipal principal)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<IEnumerable<TermsOfUseModel>>("Unknown User");
            }

            var res = await store.QueryAsync(_ => _.OrganisationId == userDataRes.Entity.OrganisationId || _.OrganisationId == null);
            return new EntityOperationResult<IEnumerable<TermsOfUseModel>>(userDataRes.Entity, res);
        }

        public async Task<EntityOperationResult<TermsOfUseModel>> CreateAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<TermsOfUseModel>("Unknown User");
            }

            model.OrganisationId = userDataRes.Entity.OrganisationId;
            model.Organisation = userDataRes.Entity.Organisation;

            try
            {
                model = await store.CreateAsync(model);
            }
            catch (System.Exception ex)
            {
                logger.LogError("Failed to create Terms of Use Model", ex);
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, "Failed to create");
            }

            if (model != null)
            {
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, model);
            }
            else
            {
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, "Something went wrong when creating model");
            }
        }

        public async Task<EntityOperationResult<TermsOfUseModel>> ReadAsync(ClaimsPrincipal principal, string touId)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<TermsOfUseModel>("Unknown User");
            }

            var tou = await store.ReadAsync(touId);
            return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, tou);
        }

        public async Task<EntityOperationResult<TermsOfUseModel>> UpdateAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<TermsOfUseModel>("Unknown User");
            }

            var authorized = await permissionService.IsAuthorizedAsync(userDataRes.Entity,
                                                                       model.Organisation,
                                                                       AccessLevels.Update);

            if (authorized)
            {
                model = await store.UpdateAsync(model);
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, model);
            }
            else
            {
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, false) { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<TermsOfUseModel>> DeleteAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<TermsOfUseModel>("Unknown User");
            }

            var authorized = await permissionService.IsAuthorizedAsync(userDataRes.Entity,
                                                                       model.Organisation,
                                                                       AccessLevels.Administer);

            if (authorized)
            {
                // check if there are any amphora referencing that.
                if (model.AppliedToAmphoras?.Count > 0)
                {
                    return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity,
                        $"You cannot delete this TOU because there are {model.AppliedToAmphoras?.Count} Amphora referencing these terms.");
                }

                try
                {
                    await store.DeleteAsync(model);
                    return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, true);
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to delete.", ex);
                    return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, "A database error occurred");
                }
            }
            else
            {
                return new EntityOperationResult<TermsOfUseModel>(userDataRes.Entity, false) { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<TermsOfUseAcceptanceModel>> AcceptAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            if (userDataRes.Failed)
            {
                return new EntityOperationResult<TermsOfUseAcceptanceModel>("Unknown User");
            }

            if (userDataRes.Entity.OrganisationId == model.OrganisationId)
            {
                return new EntityOperationResult<TermsOfUseAcceptanceModel>(userDataRes.Entity, "Cannot accept your own terms");
            }

            if (userDataRes.Entity.Organisation.TermsOfUsesAccepted.Any(_ => _.TermsOfUseId == model.Id))
            {
                // org has already accepted
                return new EntityOperationResult<TermsOfUseAcceptanceModel>(userDataRes.Entity, "Organisation has already accepted");
            }

            var authorized = await permissionService.IsAuthorizedAsync(userDataRes.Entity,
                                                                      userDataRes.Entity.Organisation, // applies on users own
                                                                      AccessLevels.Update);

            if (authorized)
            {
                var acceptanceModel = new TermsOfUseAcceptanceModel(userDataRes.Entity.Organisation, model);
                userDataRes.Entity.Organisation.TermsOfUsesAccepted.Add(acceptanceModel);
                await userDataService.UpdateAsync(principal, userDataRes.Entity);
                return new EntityOperationResult<TermsOfUseAcceptanceModel>(userDataRes.Entity, acceptanceModel);
            }
            else
            {
                return new EntityOperationResult<TermsOfUseAcceptanceModel>(userDataRes.Entity, "User needs Update permissions on org.")
                { WasForbidden = true };
            }
        }
    }
}