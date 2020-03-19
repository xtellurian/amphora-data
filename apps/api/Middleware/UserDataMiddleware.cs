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

        public UserDataMiddleware(RequestDelegate next,
                                   ILogger<UserDataMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext,
                                IUserDataService userDataService)
        {
            var id = httpContext.User.GetUserId();
            var email = httpContext.User.GetEmail();
            var name = httpContext.User.GetUserName();
            var fullName = httpContext.User.GetFullName();
            var about = httpContext.User.GetAbout();

            // check if application user data exists
            var readRes = await userDataService.ReadAsync(httpContext.User, id);
            if (!readRes.Succeeded && httpContext.User.Identity.IsAuthenticated)
            {
                // there's not user data but the user is authenticated. So create one.
                var createRes = await userDataService.CreateAsync(httpContext.User,
                    new ApplicationUserDataModel
                    {
                        Id = id,
                        UserName = name,
                        About = about,
                        ContactInformation = new ContactInformation(email, fullName)
                    });

                if (!createRes.Succeeded)
                {
                    logger.LogError($"Failed to create user data for user: {name}");
                }
            }
            else
            {
                var userData = readRes.Entity;
                var needsUpdate = false;
                // check whether we need to make updates to Full Name or About
                if (!string.Equals(userData.About, about))
                {
                    userData.About = about;
                    needsUpdate = true;
                }

                if (!string.Equals(userData.ContactInformation.FullName, fullName))
                {
                    userData.ContactInformation.FullName = fullName;
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

            await _next(httpContext);
        }
    }
}