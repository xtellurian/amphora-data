using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Extensions;
using Microsoft.Extensions.Logging;
using Amphora.Api.Models;

namespace Amphora.Api.Services
{
    public class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<Common.Models.Amphora> amphoraStore;
        private readonly IEntityStore<Organisation> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IEntityStore<Common.Models.Amphora> amphoraStore,
                               IEntityStore<Organisation> organisationStore,
                               IPermissionService permissionService,
                               IUserManager userManager,
                               ILogger<AmphoraeService> logger)
        {
            this.amphoraStore = amphoraStore;
            this.organisationStore = organisationStore;
            this.permissionService = permissionService;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<EntityOperationResult<Common.Models.Amphora>> CreateAsync(Common.Models.Amphora model, ClaimsPrincipal creator)
        {
            logger.LogInformation($"Creating new Amphora");
            if (! model.IsValidDto()) throw new NullReferenceException("Invalid Amphora Model"); 
            var user = await userManager.GetUserAsync(creator);
            if(model.OrganisationId == null) 
            {
                model.OrganisationId = user.OrganisationId;
                logger.LogInformation($"Setting Amphora OrganisationId to {user.OrganisationId}");
            }
            // check permission to create amphora
            var organisation = await organisationStore.ReadAsync(
                model.OrganisationId.AsQualifiedId(typeof(Organisation)), 
                model.OrganisationId);
            
            var isAuthorized = await permissionService.IsAuthorized(user, organisation, ResourcePermissions.Create);
            if(isAuthorized)
            {
                model = await amphoraStore.CreateAsync(model);
                await permissionService.SetIsOwner(user, model);
                return new EntityOperationResult<Common.Models.Amphora>(model);
            }
            else
            {
                return new EntityOperationResult<Common.Models.Amphora>("Unauthorized", $"Create permission required on {organisation.Id}")
                    {WasForbidden = true};
            }
        }
    }
}