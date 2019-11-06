using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{

    public class GlobalAdminAuthorizationHandler : AuthorizationHandler<GlobalAdminRequirement>
    {
        private readonly IUserService userService;
        private readonly ILogger<GlobalAdminAuthorizationHandler> logger;

        public GlobalAdminAuthorizationHandler(IUserService userService, ILogger<GlobalAdminAuthorizationHandler> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       GlobalAdminRequirement requirement)
        {
            var user = await userService.ReadUserModelAsync(context.User);
            if(user != null && user.IsGlobalAdmin.HasValue && user.IsGlobalAdmin.Value)
            {
                logger.LogWarning($"{user.Email} has been granted global admin");
                context.Succeed(requirement);
            }
        }
    }
}
