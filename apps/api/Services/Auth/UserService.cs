using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPermissionService permissionService;
        private readonly IEventPublisher eventPublisher;
        private readonly ISignInManager signInManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<OrganisationModel> orgStore,
                           IEntityStore<ApplicationUser> userStore,
                           IPermissionService permissionService,
                           IEventPublisher eventPublisher,
                           IUserManager userManager,
                           ISignInManager signInManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            UserStore = userStore;
            this.permissionService = permissionService;
            this.eventPublisher = eventPublisher;
            this.UserManager = userManager;
            this.signInManager = signInManager;
        }

        public IEntityStore<ApplicationUser> UserStore { get; }
        public IUserManager UserManager { get; protected set; }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var res = await signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            if (res.Succeeded)
            {
                // get user and set last signin time
                var user = await UserManager.FindByNameAsync(userName);
                user.LastLoggedIn = System.DateTime.UtcNow;
                await UserManager.UpdateAsync(user);
                await eventPublisher.PublishEventAsync(new SignInEvent(user));
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
            // invitation model could be null
            using (logger.BeginScope(new LoggerScope<UserService>(user)))
            {
                var existing = await UserManager.FindByIdAsync(user.Id);
                if (existing != null)
                {
                    logger.LogWarning("Duplicate User Id");
                    throw new System.ArgumentException("Duplicate User Id");
                }

                logger.LogInformation("Creating User...");
                if (invitation?.IsGlobalAdmin == true && !user.IsGlobalAdmin.HasValue)
                {
                    logger.LogWarning($"Creating global admin {user.Email}");
                    user.IsGlobalAdmin = invitation.IsGlobalAdmin;
                }

                user.LastModified = System.DateTime.UtcNow;
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByNameAsync(user.UserName);
                    if (user == null) { throw new System.Exception("Unable to retrieve user"); }
                    await eventPublisher.PublishEventAsync(new UserCreatedEvent(user));
                    // create role here
                    return new EntityOperationResult<ApplicationUser>(user, user);
                }
                else
                {
                    return new EntityOperationResult<ApplicationUser>(user, result.Errors.Select(e => e.Description));
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
                    if (currentUser == null) { return new EntityOperationResult<ApplicationUser>(currentUser, "User does not exist"); }
                    var result = await UserManager.DeleteAsync(currentUser);
                    if (result.Succeeded)
                    {
                        return new EntityOperationResult<ApplicationUser>(true);
                    }
                    else
                    {
                        return new EntityOperationResult<ApplicationUser>(currentUser, result.Errors.Select(e => e.Description));
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