using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPermissionService permissionService;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<OrganisationModel> orgStore,
                           IPermissionService permissionService,
                           IUserManager userManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.permissionService = permissionService;
            this.UserManager = userManager;

        }

        public IUserManager UserManager {get; protected set;}

        public async Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user,
                                                                               string password,
                                                                               RoleAssignment.Roles role = RoleAssignment.Roles.User)
        {
            if (user.Validate())
            {
                var org = await orgStore.ReadAsync(user.OrganisationId);
                if (org == null && !user.IsOnboarding)
                {
                    return new EntityOperationResult<IApplicationUser>("Unknown Organisation");
                }
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByNameAsync(user.UserName);
                    if (user == null) throw new System.Exception("Unable to retrieve user");
                    // create role here
                    if (!user.IsOnboarding) await permissionService.CreateOrganisationalRole(user, role, org);
                    return new EntityOperationResult<IApplicationUser>(user);
                }
                else
                {
                    return new EntityOperationResult<IApplicationUser>(result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                return new EntityOperationResult<IApplicationUser>("Invalid User");
            }

        }

        public async Task<EntityOperationResult<IApplicationUser>> DeleteAsync(IApplicationUser user)
        {
            // todo - permissions
            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new EntityOperationResult<IApplicationUser>();
            }
            else
            {
                return new EntityOperationResult<IApplicationUser>(result.Errors.Select(e => e.Description));
            }
        }
    }
}