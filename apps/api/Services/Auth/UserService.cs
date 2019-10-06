using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Users;
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
        private readonly IEmailLimitingService emailLimitingService;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<OrganisationModel> orgStore,
                           IPermissionService permissionService,
                           IEmailLimitingService emailLimitingService,
                           IUserManager userManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.permissionService = permissionService;
            this.emailLimitingService = emailLimitingService;
            this.UserManager = userManager;

        }

        public IUserManager UserManager { get; protected set; }

        public async Task<EntityOperationResult<ApplicationUser>> CreateAsync(ApplicationUser user,
                                                                              string password)
        {
            var existing = await UserManager.FindByIdAsync(user.Id);
            if (existing != null)
            {
                throw new System.ArgumentException("Duplicate User Id");
            }
            if(!emailLimitingService.CanSignup(user.Email))
            {
                return new EntityOperationResult<ApplicationUser>($"{user.Email} is not authorized to signup");
            }

            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                user = await UserManager.FindByNameAsync(user.UserName);
                if (user == null) throw new System.Exception("Unable to retrieve user");
                // create role here
                return new EntityOperationResult<ApplicationUser>(user);
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>(result.Errors.Select(e => e.Description));
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> DeleteAsync(IUser user)
        {
            // todo - permissions
            var appUser = await UserManager.FindByIdAsync(user.Id);
            if(appUser == null) return new EntityOperationResult<ApplicationUser>();
            var result = await UserManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return new EntityOperationResult<ApplicationUser>();
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>(result.Errors.Select(e => e.Description));
            }
        }

        public async Task<ApplicationUser> ReadUserModelAsync(ClaimsPrincipal principal)
        {
            var appUser = await UserManager.GetUserAsync(principal);
            return appUser;
        }
    }
}