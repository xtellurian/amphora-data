using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class RestrictionService : IRestrictionService
    {
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly ILogger<RestrictionService> logger;

        public RestrictionService(IEntityStore<RestrictionModel> store,
                                  IUserDataService userDataService,
                                  IPermissionService permissionService,
                                  ILogger<RestrictionService> logger)
        {
            Store = store;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public IEntityStore<RestrictionModel> Store { get; }

        public async Task<EntityOperationResult<RestrictionModel>> CreateAsync(ClaimsPrincipal principal, RestrictionModel restriction)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<RestrictionModel>(false);
            }

            var userData = userReadRes.Entity;

            using (logger.BeginScope(new LoggerScope<PermissionService>(userData)))
            {
                if (restriction.SourceOrganisationId == restriction.TargetOrganisationId)
                {
                    return new EntityOperationResult<RestrictionModel>(userData, "You cannot restrict your own organisation");
                }

                // prevent duplication targetting org
                if (restriction.Scope == RestrictionScope.Organisation)
                {
                    if (await Store.CountAsync(_ => _.TargetOrganisationId == restriction.TargetOrganisationId &&
                        _.Scope == RestrictionScope.Organisation) > 0)
                    {
                        return new EntityOperationResult<RestrictionModel>(userData, "Restriction already exists");
                    }
                }
                else if (restriction.Scope == RestrictionScope.Amphora)
                {
                    // prevent duplication at Amphora level
                    if (await Store.CountAsync(_ => _.TargetOrganisationId == restriction.TargetOrganisationId &&
                        _.Scope == RestrictionScope.Amphora &&
                        _.SourceAmphoraId == restriction.SourceAmphoraId) > 0)
                    {
                        return new EntityOperationResult<RestrictionModel>(userData, "Restriction already exists");
                    }
                }

                if (restriction.SourceAmphoraId != null)
                {
                    // create restriciton on an Amphora
                    return await CreateAmphoraRestriction(userData, restriction);
                }
                else if (restriction.SourceOrganisationId != null)
                {
                    return await CreateOrganisationRestriction(userData, restriction);
                }
                else
                {
                    throw new System.InvalidOperationException("Unknown Restriction Configuration");
                }
            }
        }

        public async Task<EntityOperationResult<RestrictionModel>> DeleteAsync(ClaimsPrincipal principal, string restrictionId)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<RestrictionModel>(false);
            }

            var userData = userReadRes.Entity;

            using (logger.BeginScope(new LoggerScope<PermissionService>(userData)))
            {
                var model = await Store.ReadAsync(restrictionId);
                if (model == null)
                {
                    return new EntityOperationResult<RestrictionModel>(userData);
                }

                if (await permissionService.IsAuthorizedAsync(userData, model.SourceOrganisation, AccessLevels.Update))
                {
                    logger.LogInformation($"Deleting Restriction Id {model.Id}");
                    await Store.DeleteAsync(model);
                    return new EntityOperationResult<RestrictionModel>(userData, true);
                }
                else
                {
                    logger.LogWarning($"User {userData.UserName} attemped to delete restriction {model.Id}, but permission not granted");
                    return new EntityOperationResult<RestrictionModel>(userData, false) { WasForbidden = true };
                }
            }
        }

        private async Task<EntityOperationResult<RestrictionModel>> CreateAmphoraRestriction(IUser user, RestrictionModel restriction)
        {
            if (await permissionService.IsAuthorizedAsync(user, restriction.SourceAmphora, AccessLevels.Update))
            {
                restriction = await Store.CreateAsync(restriction);
                return new EntityOperationResult<RestrictionModel>(user, restriction);
            }
            else
            {
                return new EntityOperationResult<RestrictionModel>(user, $"{user.UserName} has needs update permission") { WasForbidden = true };
            }
        }

        private async Task<EntityOperationResult<RestrictionModel>> CreateOrganisationRestriction(IUser user, RestrictionModel restriction)
        {
            if (await permissionService.IsAuthorizedAsync(user, restriction.SourceOrganisation, AccessLevels.Update))
            {
                restriction = await Store.CreateAsync(restriction);
                return new EntityOperationResult<RestrictionModel>(user, restriction);
            }
            else
            {
                return new EntityOperationResult<RestrictionModel>(user, $"{user.UserName} has needs update permission") { WasForbidden = true };
            }
        }
    }
}