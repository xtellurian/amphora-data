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
                        ContactInformation = new ContactInformation(email, name)
                    });

                if (!createRes.Succeeded)
                {
                    logger.LogError($"Failed to create user data for user: {name}");
                }
            }

            await _next(httpContext);
        }
    }
}