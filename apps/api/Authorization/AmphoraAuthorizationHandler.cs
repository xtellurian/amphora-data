using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Authorization
{
    public class AmphoraAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Amphora.Common.Models.Amphora>
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
                                                    OperationAuthorizationRequirement requirement,
                                                       Amphora.Common.Models.Amphora entity)
        {
            var user = await userManager.GetUserAsync(context.User);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, entity, requirement.Name);

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
