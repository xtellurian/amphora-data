using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class RestrictionService : IRestrictionService
    {
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly ILogger<RestrictionService> logger;

        public RestrictionService(IEntityStore<RestrictionModel> store,
                                  IUserService userService,
                                  IPermissionService permissionService,
                                  ILogger<RestrictionService> logger)
        {
            Store = store;
            this.userService = userService;
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public IEntityStore<RestrictionModel> Store { get; }

        public async Task<EntityOperationResult<RestrictionModel>> CreateAsync(ClaimsPrincipal principal, RestrictionModel restriction)
        {
            var user = await userService.ReadUserModelAsync(principal);

            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                if (restriction.SourceAmphoraId != null)
                {
                    // create restriciton on an Amphora
                    return await CreateAmphoraRestriction(user, restriction);
                }
                else if (restriction.SourceOrganisationId != null)
                {
                    return await CreateOrganisationRestriction(user, restriction);
                }
                else
                {
                    throw new System.InvalidOperationException("Unknown Restriction COnfiguration");
                }
            }
        }

        public async Task<EntityOperationResult<RestrictionModel>> DeleteAsync(ClaimsPrincipal principal, string restrictionId)
        {
            var user = await userService.ReadUserModelAsync(principal);

            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                var model = await Store.ReadAsync(restrictionId);
                if (model == null)
                {
                    return new EntityOperationResult<RestrictionModel>(user, 404);
                }

                if (await permissionService.IsAuthorizedAsync(user, model.SourceOrganisation, AccessLevels.Update))
                {
                    logger.LogInformation($"Deleting Restriction Id {model.Id}");
                    await Store.DeleteAsync(model);
                    return new EntityOperationResult<RestrictionModel>(user, 200, true);
                }
                else
                {
                    logger.LogWarning($"User {user.UserName} attemped to delete restriction {model.Id}, but permission not granted");
                    return new EntityOperationResult<RestrictionModel>(user, 403, false) { WasForbidden = true };
                }
            }
        }

        private async Task<EntityOperationResult<RestrictionModel>> CreateAmphoraRestriction(ApplicationUser user, RestrictionModel restriction)
        {
            if (await permissionService.IsAuthorizedAsync(user, restriction.SourceAmphora, AccessLevels.Update))
            {
                restriction = await Store.CreateAsync(restriction);
                return new EntityOperationResult<RestrictionModel>(user, restriction);
            }
            else
            {
                return new EntityOperationResult<RestrictionModel>(user, 403, $"{user.UserName} has needs update permission") { WasForbidden = true };
            }
        }

        private async Task<EntityOperationResult<RestrictionModel>> CreateOrganisationRestriction(ApplicationUser user, RestrictionModel restriction)
        {
            if (await permissionService.IsAuthorizedAsync(user, restriction.SourceOrganisation, AccessLevels.Update))
            {
                restriction = await Store.CreateAsync(restriction);
                return new EntityOperationResult<RestrictionModel>(user, restriction);
            }
            else
            {
                return new EntityOperationResult<RestrictionModel>(user, 403, $"{user.UserName} has needs update permission") { WasForbidden = true };
            }
        }
    }
}