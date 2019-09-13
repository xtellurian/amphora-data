using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class AmphoraAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement, Amphora.Common.Models.AmphoraModel>
    {
        private readonly ILogger<AmphoraAuthorizationHandler> logger;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;

        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.2
        public AmphoraAuthorizationHandler(ILogger<AmphoraAuthorizationHandler> logger,
                                           IPermissionService permissionService,
                                           IUserManager userManager)
        {
            this.logger = logger;
            this.permissionService = permissionService;
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    AuthorizationRequirement requirement,
                                                       Amphora.Common.Models.AmphoraModel entity)
        {
            var user = await userManager.GetUserAsync(context.User);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, entity, requirement.MinimumLevel);

            if (isAuthorized)
            {
                context.Succeed(requirement);
                logger.LogInformation($"Access granted to {entity.Id}");
            }
            else
            {
                logger.LogInformation($"Access Denied to {entity.Id}");
            }
        }
    }
}
