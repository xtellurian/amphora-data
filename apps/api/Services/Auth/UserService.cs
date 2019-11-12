using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Users;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPermissionService permissionService;
        private readonly ISignInManager signInManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<OrganisationModel> orgStore,
                           IPermissionService permissionService,
                           IUserManager userManager,
                           ISignInManager signInManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.permissionService = permissionService;
            this.UserManager = userManager;
            this.signInManager = signInManager;
        }

        public IUserManager UserManager { get; protected set; }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var res = await signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            if(res.Succeeded)
            {
                // get user and set last signin time
                var user = await UserManager.FindByNameAsync(userName);
                user.LastLoggedIn = System.DateTime.UtcNow;
                await UserManager.UpdateAsync(user);
            }
            return res;
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return this.signInManager.IsSignedIn(principal);
        }

        public async Task<EntityOperationResult<ApplicationUser>> CreateAsync(ApplicationUser user,
                                                                              InvitationModel invitation,
                                                                              string password)
        {
            using (logger.BeginScope(new LoggerScope<UserService>(user)))
            {
                var existing = await UserManager.FindByIdAsync(user.Id);
                if (existing != null)
                {
                    logger.LogWarning("Duplicate User Id");
                    throw new System.ArgumentException("Duplicate User Id");
                }

                logger.LogInformation("Creating User...");
                if (invitation.IsGlobalAdmin.HasValue && invitation.IsGlobalAdmin.Value)
                {
                    logger.LogWarning($"Creating global admin {user.Email}");
                    user.IsGlobalAdmin = invitation.IsGlobalAdmin;
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
        }

        public async Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user)
        {
            var currentUser = await UserManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<UserService>(currentUser)))
            {
                if (currentUser.Id == user.Id)
                {
                    logger.LogWarning("User is deleting self");
                    if (currentUser == null) return new EntityOperationResult<ApplicationUser>();
                    var result = await UserManager.DeleteAsync(currentUser);
                    if (result.Succeeded)
                    {
                        return new EntityOperationResult<ApplicationUser>();
                    }
                    else
                    {
                        return new EntityOperationResult<ApplicationUser>(result.Errors.Select(e => e.Description));
                    }
                }
                else
                {
                    logger.LogCritical($"User is deleting other user, other username: {user.UserName}");
                    return new EntityOperationResult<ApplicationUser> { WasForbidden = true };
                }
            }
        }

        public async Task<ApplicationUser> ReadUserModelAsync(ClaimsPrincipal principal)
        {
            var appUser = await UserManager.GetUserAsync(principal);
            return appUser;
        }
    }
}