using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Authorization
{
    public class AmphoraAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Amphora.Common.Models.Amphora>
    {
        private readonly ILogger<AmphoraAuthorizationHandler> logger;

        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.2
        public AmphoraAuthorizationHandler(ILogger<AmphoraAuthorizationHandler> logger)
        {
            this.logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Amphora.Common.Models.Amphora entity)
        {
            logger.LogInformation("Automatically giving read access");
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
