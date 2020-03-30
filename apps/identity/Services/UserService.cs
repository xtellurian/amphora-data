using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Platform;
using Amphora.Identity.Contracts;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEventPublisher eventPublisher;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(ILogger<UserService> logger,
                           IEventPublisher eventPublisher,
                           UserManager<ApplicationUser> userManager)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.eventPublisher = eventPublisher ?? throw new System.ArgumentNullException(nameof(eventPublisher));
            this.userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
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
                    return new EntityOperationResult<ApplicationUser>(user, result.Errors.Select(e => e.Description).ToArray());
                }
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user)
        {
            var currentUser = await userManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<UserService>(currentUser)))
            {
                if (currentUser != null && currentUser.Id == user.Id)
                {
                    logger.LogWarning("User is deleting self");
                    var result = await userManager.DeleteAsync(currentUser);
                    if (result.Succeeded)
                    {
                        return new EntityOperationResult<ApplicationUser>(true);
                    }
                    else
                    {
                        return new EntityOperationResult<ApplicationUser>(currentUser, result.Errors.Select(e => e.Description).ToArray());
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
                    return new EntityOperationResult<ApplicationUser>(currentUser, result.Errors.Select(_ => _.Description).ToArray());
                }
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>("Wrong User ID") { WasForbidden = true };
            }
        }
    }
}