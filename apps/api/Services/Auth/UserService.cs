using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
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

        public IUserManager UserManager { get; protected set; }

        public async Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user,
                                                                               string password)
        {
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                user = await UserManager.FindByNameAsync(user.UserName);
                if (user == null) throw new System.Exception("Unable to retrieve user");
                // create role here
                return new EntityOperationResult<IApplicationUser>(user);
            }
            else
            {
                return new EntityOperationResult<IApplicationUser>(result.Errors.Select(e => e.Description));
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