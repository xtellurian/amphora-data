using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPermissionService permissionService;
        private readonly IEventPublisher eventPublisher;
        private readonly IIdentityService identityServerService;
        private readonly ISignInManager signInManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<OrganisationModel> orgStore,
                           IEntityStore<ApplicationUser> userStore,
                           IPermissionService permissionService,
                           IEventPublisher eventPublisher,
                           IIdentityService identityServerService,
                           ISignInManager signInManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            UserStore = userStore;
            this.permissionService = permissionService;
            this.eventPublisher = eventPublisher;
            this.identityServerService = identityServerService;
            this.signInManager = signInManager;
        }

        public IEntityStore<ApplicationUser> UserStore { get; }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure, bool isAcquiringToken = false)
        {
            var res = await signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            if (res.Succeeded)
            {
                // get user and set last signin time
                var user = await UserStore.Query(_ => _.NormalizedUserName == userName.ToUpper()).FirstOrDefaultAsync();
                user.LastLoggedIn = System.DateTime.UtcNow;
                await UserStore.UpdateAsync(user);
                if (!isAcquiringToken) // don't publish the token acquisition logins
                {
                    await eventPublisher.PublishEventAsync(new SignInEvent(user, isAcquiringToken));
                }
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
                var existing = await UserStore.ReadAsync(user.Id);
                if (existing != null)
                {
                    logger.LogWarning("Duplicate User Id");
                    throw new System.ArgumentException("Duplicate User Id");
                }

                logger.LogInformation("Creating User...");

                user.LastModified = System.DateTime.UtcNow;
                var result = await identityServerService.CreateUser(user, password);
                if (result.Succeeded)
                {
                    user = result.Entity;
                    await eventPublisher.PublishEventAsync(new UserCreatedEvent(user));
                    // create role here
                    return new EntityOperationResult<ApplicationUser>(user, user);
                }
                else
                {
                    return new EntityOperationResult<ApplicationUser>(user, result.Errors);
                }
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user)
        {
            var currentUser = await this.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<UserService>(currentUser)))
            {
                if (currentUser.Id == user.Id)
                {
                    logger.LogWarning("User is deleting self");
                    if (currentUser == null) { return new EntityOperationResult<ApplicationUser>(currentUser, "User does not exist"); }
                    var result = await identityServerService.DeleteUser(principal, user);
                    return result;
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
            var userId = principal.GetUserId();
            if (userId == null)
            {
                return null;
            }
            else
            {
                return await UserStore.ReadAsync(userId);
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> UpdateAsync(ClaimsPrincipal principal, ApplicationUser user)
        {
            var userId = principal.GetUserId();
            if (user.Id != userId)
            {
                return new EntityOperationResult<ApplicationUser>(false);
            }

            user = await this.UserStore.UpdateAsync(user);
            if (user != null)
            {
                return new EntityOperationResult<ApplicationUser>(user, 200, true);
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>(false);
            }
        }
    }
}