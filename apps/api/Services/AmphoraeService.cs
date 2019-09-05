using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services
{
    public class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<Common.Models.Amphora> amphoraStore;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IEntityStore<Common.Models.Amphora> amphoraStore,
                               IPermissionService permissionService,
                               IUserManager userManager,
                               ILogger<AmphoraeService> logger)
        {
            this.amphoraStore = amphoraStore;
            this.permissionService = permissionService;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<Common.Models.Amphora> CreateAsync(Common.Models.Amphora model, ClaimsPrincipal creator)
        {
            logger.LogInformation($"Creating new Amphora");
            if (! model.IsValidDto()) throw new NullReferenceException("OrganisationId cannot be null"); 
            model = await amphoraStore.CreateAsync(model);
            var user = await userManager.GetUserAsync(creator);
            await permissionService.SetIsOwner(user, model);
            return model;
        }
    }
}