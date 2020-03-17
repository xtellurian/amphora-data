using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class GlobalAdminAuthorizationHandler : AuthorizationHandler<GlobalAdminRequirement>
    {
        public GlobalAdminAuthorizationHandler()
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             GlobalAdminRequirement requirement)
        {
            if (context.User.IsGlobalAdmin())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
