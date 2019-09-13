using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Extensions;
using Microsoft.Extensions.Logging;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Contracts;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<OrganisationModel> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraeService> logger;

        public IEntityStore<Common.Models.AmphoraModel> AmphoraStore { get; }

        public AmphoraeService(IEntityStore<Common.Models.AmphoraModel> amphoraStore,
                               IEntityStore<OrganisationModel> organisationStore,
                               IPermissionService permissionService,
                               IUserManager userManager,
                               ILogger<AmphoraeService> logger)
        {
            AmphoraStore = amphoraStore;
            this.organisationStore = organisationStore;
            this.permissionService = permissionService;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<EntityOperationResult<Common.Models.AmphoraModel>> CreateAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel model)
        {
            logger.LogInformation($"Creating new Amphora");
            var user = await userManager.GetUserAsync(principal);
            if (string.IsNullOrEmpty(model.OrganisationId)) model.OrganisationId = user.OrganisationId;
            if (!model.IsValidDto()) throw new NullReferenceException("Invalid Amphora Model");
            if (model.OrganisationId == null)
            {
                model.OrganisationId = user.OrganisationId;
                logger.LogInformation($"Setting Amphora OrganisationId to {user.OrganisationId}");
            }
            // check permission to create amphora
            var organisation = await organisationStore.ReadAsync(
                model.OrganisationId.AsQualifiedId(typeof(OrganisationModel)),
                model.OrganisationId);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Create);
            if (isAuthorized)
            {
                model = await AmphoraStore.CreateAsync(model);
                await permissionService.SetIsOwner(user, model);
                return new EntityOperationResult<Common.Models.AmphoraModel>(model);
            }
            else
            {
                return new EntityOperationResult<Common.Models.AmphoraModel>("Unauthorized", $"Create permission required on {organisation.Id}")
                { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.AmphoraModel>> ReadAsync(ClaimsPrincipal principal, string id, string orgId = null)
        {
            logger.LogDebug($"Reading Amphora {id}");
            var (user, entity) = await GetUserAndEntityAsync(principal, id, orgId);
            if (entity == null)
            {
                logger.LogError($"{id} Not Found");
                return new EntityOperationResult<Common.Models.AmphoraModel>($"{id} Not Found");
            }
            if (entity.IsPublic)
            {
                logger.LogInformation($"Permission granted to public entity {entity.Id}");
                return new EntityOperationResult<Common.Models.AmphoraModel>(entity);
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Read);
            if (authorized)
            {
                return new EntityOperationResult<Common.Models.AmphoraModel>(entity);
            }
            else
            {
                return new EntityOperationResult<Common.Models.AmphoraModel>("Denied") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity)
        {
            logger.LogDebug($"Updating Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, entity.OrganisationId);
            if (entity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<Common.Models.AmphoraModel>($"{entity.Id} Not Found");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Update);
            if (authorized)
            {
                if (string.Equals(entity.OrganisationId, existingEntity.OrganisationId))
                {
                    var result = await AmphoraStore.UpdateAsync(entity);
                    return new EntityOperationResult<Common.Models.AmphoraModel>(result);
                }
                else
                {
                    return new EntityOperationResult<Common.Models.AmphoraModel>("Cannot change organisation id");
                }
            }
            else
            {
                return new EntityOperationResult<Common.Models.AmphoraModel>("Unauthorized") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity)
        {
            logger.LogInformation($"Deleting Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, entity.OrganisationId);
            if (existingEntity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<Common.Models.AmphoraModel>($"{entity.Id} Not Found");
            }
            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Delete);
            if (authorized)
            {
                await AmphoraStore.DeleteAsync(entity);
                return new EntityOperationResult<Common.Models.AmphoraModel>();
            }
            else
            {
                return new EntityOperationResult<Common.Models.AmphoraModel>("Unauthorized") { WasForbidden = true };
            }
        }

        private async Task<(IApplicationUser user, Common.Models.AmphoraModel entity)> GetUserAndEntityAsync(ClaimsPrincipal principal, string id, string orgId)
        {
            id = id.AsQualifiedId<Common.Models.AmphoraModel>();
            var user = await userManager.GetUserAsync(principal);
            var entity = await AmphoraStore.ReadAsync(id, orgId);
            return (user, entity);
        }
    }
}