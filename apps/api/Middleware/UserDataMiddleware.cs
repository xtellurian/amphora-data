using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class UserDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserDataMiddleware> logger;
        private System.TimeSpan lastSeenFilter = new System.TimeSpan(6, 0, 0); // 6 hours

        public UserDataMiddleware(RequestDelegate next,
                                   ILogger<UserDataMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext,
                                IUserDataService userDataService,
                                IDateTimeProvider dtProvider)
        {
            var now = dtProvider.UtcNow;
            // is the user authenticated
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var id = httpContext.User.GetUserId();
                var email = httpContext.User.GetEmail();
                var emailConfirmed = httpContext.User.IsEmailConfirmed();
                var name = httpContext.User.GetUserName();
                var fullName = httpContext.User.GetFullName();
                var about = httpContext.User.GetAbout();
                // check if application user data exists
                var readRes = await userDataService.ReadAsync(httpContext.User, id);
                if (!readRes.Succeeded)
                {
                    // there's not user data but the user is authenticated. So create one.
                    var createRes = await userDataService.CreateAsync(httpContext.User,
                        new ApplicationUserDataModel
                        {
                            Id = id,
                            UserName = name,
                            About = about,
                            ContactInformation = new ContactInformation(email, fullName),
                            LastSeen = now
                        });

                    if (!createRes.Succeeded)
                    {
                        logger.LogError($"Failed to create user data for user: {name}");
                    }
                }
                else if (readRes.Succeeded)
                {
                    var userData = readRes.Entity;
                    userData.ContactInformation ??= new ContactInformation();
                    var needsUpdate = false;
                    // check whether we need to make updates to Full Name or About
                    if (!string.Equals(userData.About, about))
                    {
                        userData.About = about;
                        needsUpdate = true;
                    }

                    // if LastSeen is null or larger than the filter time, then update it.
                    if (userData.LastSeen == null || (userData.LastSeen + lastSeenFilter > now))
                    {
                        userData.LastSeen = now;
                        needsUpdate = true;
                    }

                    // check username
                    if (!string.Equals(userData.UserName, name))
                    {
                        userData.UserName = name;
                        needsUpdate = true;
                    }

                    if (!string.Equals(userData.ContactInformation?.FullName, fullName))
                    {
                        userData.ContactInformation.FullName = fullName;
                        userData.ContactInformation.Email = email;
                        userData.ContactInformation.EmailConfirmed = emailConfirmed;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        try
                        {
                            var updateRes = await userDataService.UpdateAsync(httpContext.User, userData);
                            if (updateRes.Succeeded)
                            {
                                logger.LogInformation($"Updated User Data for {email}");
                            }
                            else
                            {
                                logger.LogError($"Failed to update User Data for {email}");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            logger.LogCritical($"Exception thrown in UserDataMiddleware when updating user data for {email}. Exception: {ex.Message}");
                        }
                    }
                }
            }

            await _next(httpContext);
        }
    }
}