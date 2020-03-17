using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class AmphoraAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement, AmphoraModel>
    {
        private readonly ILogger<AmphoraAuthorizationHandler> logger;
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;

        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.2
        public AmphoraAuthorizationHandler(ILogger<AmphoraAuthorizationHandler> logger,
                                           IPermissionService permissionService,
                                           IUserDataService userDataService)
        {
            this.logger = logger;
            this.permissionService = permissionService;
            this.userDataService = userDataService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             AuthorizationRequirement requirement,
                                                             AmphoraModel entity)
        {
            var userDataRes = await userDataService.ReadAsync(context.User);
            if (!userDataRes.Succeeded)
            {
                return;
            }

            var userData = userDataRes.Entity;

            using (logger.BeginScope(new LoggerScope<AmphoraAuthorizationHandler>(userData)))
            {
                var isAuthorized = await permissionService.IsAuthorizedAsync(userData, entity, requirement.MinimumLevel);

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
