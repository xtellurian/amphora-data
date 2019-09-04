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
        private readonly IEntityStore<ResourceAuthorization> entityStore;
        private readonly IUserManager<ApplicationUser> userManager;

        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.2
        public AmphoraAuthorizationHandler(ILogger<AmphoraAuthorizationHandler> logger,
                                           IEntityStore<ResourceAuthorization> entityStore,
                                           IUserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
            this.entityStore = entityStore;
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    OperationAuthorizationRequirement requirement,
                                                       Amphora.Common.Models.Amphora entity)
        {
            var userEntity = await userManager.GetUserAsync(context.User);

            var matching = await entityStore.QueryAsync(p => 
                string.Equals(p.ResourcePermission, requirement.Name)
                && string.Equals(p.TargetResourceId, entity.Id) 
                && string.Equals(p.UserId, userEntity.Id )
                );
            if(matching == null || matching.Count() == 0)
            {
                logger.LogInformation($"Access Denied to {entity.Id}");
            }
            else
            {
                context.Succeed(requirement);
                logger.LogInformation($"Access granted to {entity.Id}");
            }
        }
    }
}
