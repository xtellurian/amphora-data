using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services
{
    public class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<Common.Models.Amphora> amphoraStore;
        private readonly IEntityStore<Common.Models.ResourceAuthorization> resourceAuthorizationStore;
        private readonly IUserManager<ApplicationUser> userManager;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IEntityStore<Common.Models.Amphora> amphoraStore,
                               IEntityStore<Common.Models.ResourceAuthorization> resourceAuthorizationStore,
                               IUserManager<ApplicationUser> userManager,
                               ILogger<AmphoraeService> logger)
        {
            this.amphoraStore = amphoraStore;
            this.resourceAuthorizationStore = resourceAuthorizationStore;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<Common.Models.Amphora> CreateAsync(Common.Models.Amphora model, ClaimsPrincipal creator)
        {
            logger.LogInformation($"Creating new Amphora");
            model = await amphoraStore.CreateAsync(model);
            var user = await userManager.GetUserAsync(creator);
            var read = new ResourceAuthorization(user.Id, model, ResourcePermissions.Read);
            read = await resourceAuthorizationStore.CreateAsync(read);
            var update = new ResourceAuthorization(user.Id, model, ResourcePermissions.Update);
            update = await resourceAuthorizationStore.CreateAsync(update);
            var delete = new ResourceAuthorization(user.Id, model, ResourcePermissions.Delete);
            delete = await resourceAuthorizationStore.CreateAsync(delete);

            return model;
        }
    }
}