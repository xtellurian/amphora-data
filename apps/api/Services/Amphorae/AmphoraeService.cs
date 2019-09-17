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
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<OrganisationModel> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraeService> logger;

        public IEntityStore<AmphoraModel> AmphoraStore { get; }

        public AmphoraeService(IEntityStore<AmphoraModel> amphoraStore,
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
        public async Task<EntityOperationResult<T>> CreateAsync<T>(ClaimsPrincipal principal, T model) where T: AmphoraModel
        {
            logger.LogInformation($"Creating new Amphora");
            var user = await userManager.GetUserAsync(principal);
            // set the required fields
            if (string.IsNullOrEmpty(model.OrganisationId)) model.OrganisationId = user.OrganisationId;
            model.CreatedBy = user.Id;
            model.CreatedDate = DateTime.UtcNow;

            if (!model.IsValid()) throw new NullReferenceException("Invalid Amphora Model");
            
            // check permission to create amphora
            var organisation = await organisationStore.ReadAsync(
                model.OrganisationId.AsQualifiedId(typeof(OrganisationModel)),
                model.OrganisationId);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Create);
            if (isAuthorized)
            {
                model = await AmphoraStore.CreateAsync(model);
                // await searchService.Reindex();
                return new EntityOperationResult<T>(model);
            }
            else
            {
                return new EntityOperationResult<T>("Unauthorized", $"Create permission required on {organisation.Id}")
                { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<T>> ReadAsync<T>(ClaimsPrincipal principal, string id, string orgId = null) where T: AmphoraModel
        {
            logger.LogDebug($"Reading Amphora {id}");
            var (user, entity) = await GetUserAndEntityAsync<T>(principal, id, orgId);
            if (entity == null)
            {
                logger.LogError($"{id} Not Found");
                return new EntityOperationResult<T>($"{id} Not Found");
            }
            if (entity.IsPublic)
            {
                logger.LogInformation($"Permission granted to public entity {entity.Id}");
                return new EntityOperationResult<T>(entity);
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Read);
            if (authorized)
            {
                return new EntityOperationResult<T>(entity);
            }
            else
            {
                return new EntityOperationResult<T>("Denied") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            logger.LogDebug($"Updating Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync<AmphoraModel>(principal, entity.Id, entity.OrganisationId);
            if (entity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<AmphoraModel>($"{entity.Id} Not Found");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Update);
            if (authorized)
            {
                if (string.Equals(entity.OrganisationId, existingEntity.OrganisationId))
                {
                    var result = await AmphoraStore.UpdateAsync(entity);
                    return new EntityOperationResult<AmphoraModel>(result);
                }
                else
                {
                    return new EntityOperationResult<AmphoraModel>("Cannot change organisation id");
                }
            }
            else
            {
                return new EntityOperationResult<AmphoraModel>("Unauthorized") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            logger.LogInformation($"Deleting Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync<AmphoraModel>(principal, entity.Id, entity.OrganisationId);
            if (existingEntity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<AmphoraModel>($"{entity.Id} Not Found");
            }
            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Delete);
            if (authorized)
            {
                await AmphoraStore.DeleteAsync(entity);
                return new EntityOperationResult<AmphoraModel>();
            }
            else
            {
                return new EntityOperationResult<AmphoraModel>("Unauthorized") { WasForbidden = true };
            }
        }

        private async Task<(IApplicationUser user, T entity)> GetUserAndEntityAsync<T>(ClaimsPrincipal principal, string id, string orgId) where T: AmphoraModel
        {
            id = id.AsQualifiedId<AmphoraModel>();
            var user = await userManager.GetUserAsync(principal);
            var entity = await AmphoraStore.ReadAsync<T>(id, orgId);
            return (user, entity);
        }
    }
}