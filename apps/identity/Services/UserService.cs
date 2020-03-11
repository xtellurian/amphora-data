using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEventPublisher eventPublisher;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<ApplicationUser> userStore,
                           IEventPublisher eventPublisher,
                           UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            UserStore = userStore ?? throw new System.ArgumentNullException(nameof(userStore));
            this.eventPublisher = eventPublisher ?? throw new System.ArgumentNullException(nameof(eventPublisher));
            this.userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new System.ArgumentNullException(nameof(signInManager));
        }

        public IEntityStore<ApplicationUser> UserStore { get; }

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
                var existing = await userManager.FindByIdAsync(user.Id);
                if (existing != null)
                {
                    logger.LogWarning("Duplicate User Id");
                    throw new System.ArgumentException("Duplicate User Id");
                }

                logger.LogInformation("Creating User...");

                user.LastModified = System.DateTime.UtcNow;
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    user = await userManager.FindByNameAsync(user.UserName);
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
            var currentUser = await userManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<UserService>(currentUser)))
            {
                if (currentUser.Id == user.Id)
                {
                    logger.LogWarning("User is deleting self");
                    if (currentUser == null) { return new EntityOperationResult<ApplicationUser>(currentUser, "User does not exist"); }
                    var result = await userManager.DeleteAsync(currentUser);
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
            var appUser = await userManager.GetUserAsync(principal);
            return appUser;
        }

        public async Task<EntityOperationResult<ApplicationUser>> UpdateAsync(ClaimsPrincipal principal, ApplicationUser user)
        {
            var currentUser = await userManager.GetUserAsync(principal);
            if (currentUser != null && user.Id == currentUser.Id)
            {
                logger.LogInformation($"Updating user: {user.UserName}");
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new EntityOperationResult<ApplicationUser>(currentUser, user);
                }
                else
                {
                    return new EntityOperationResult<ApplicationUser>(currentUser, result.Errors.Select(_ => _.Description));
                }
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>(currentUser, 403, "Wrong User ID") { WasForbidden = true };
            }
        }
    }
}