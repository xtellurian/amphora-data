using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class AmphoraAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement, AmphoraModel>
    {
        private readonly ILogger<AmphoraAuthorizationHandler> logger;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.2
        public AmphoraAuthorizationHandler(ILogger<AmphoraAuthorizationHandler> logger,
                                           IPermissionService permissionService,
                                           IUserService userService)
        {
            this.logger = logger;
            this.permissionService = permissionService;
            this.userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    AuthorizationRequirement requirement,
                                                       AmphoraModel entity)
        {
            var user = await userService.ReadUserModelAsync(context.User);
            using (logger.BeginScope(new LoggerScope<AmphoraAuthorizationHandler>(user)))
            {
                var isAuthorized = await permissionService.IsAuthorizedAsync(user, entity, requirement.MinimumLevel);

                if (isAuthorized)
                {
                    context.Succeed(requirement);
                    logger.LogInformation($"Access granted to {entity.Id}");
                }
                else
                {
                    logger.LogWarning($"Access Denied to {entity.Id}");
                }
            }
        }
    }
}
