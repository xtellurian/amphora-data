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
        public async Task<EntityOperationResult<Common.Models.Amphora>> CreateAsync(ClaimsPrincipal principal, Common.Models.Amphora model)
        {
            logger.LogInformation($"Creating new Amphora");
            if (!model.IsValidDto()) throw new NullReferenceException("Invalid Amphora Model");
            var user = await userManager.GetUserAsync(principal);
            if (model.OrganisationId == null)
            {
                model.OrganisationId = user.OrganisationId;
                logger.LogInformation($"Setting Amphora OrganisationId to {user.OrganisationId}");
            }
            // check permission to create amphora
            var organisation = await organisationStore.ReadAsync(
                model.OrganisationId.AsQualifiedId(typeof(Organisation)),
                model.OrganisationId);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Create);
            if (isAuthorized)
            {
                model = await amphoraStore.CreateAsync(model);
                await permissionService.SetIsOwner(user, model);
                return new EntityOperationResult<Common.Models.Amphora>(model);
            }
            else
            {
                return new EntityOperationResult<Common.Models.Amphora>("Unauthorized", $"Create permission required on {organisation.Id}")
                { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.Amphora>> ReadAsync(ClaimsPrincipal principal, string id, string orgId = null)
        {
            logger.LogDebug($"Reading Amphora {id}");
            var (user, entity) = await GetUserAndEntityAsync(principal, id, orgId);
            if (entity == null)
            {
                logger.LogError($"{id} Not Found");
                return new EntityOperationResult<Common.Models.Amphora>($"{id} Not Found");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Read);
            if (authorized)
            {
                return new EntityOperationResult<Common.Models.Amphora>(entity);
            }
            else
            {
                return new EntityOperationResult<Common.Models.Amphora>("Denied") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.Amphora>> UpdateAsync(ClaimsPrincipal principal, Common.Models.Amphora entity)
        {
            logger.LogDebug($"Updating Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, entity.OrganisationId);
            if (entity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<Common.Models.Amphora>($"{entity.Id} Not Found");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Update);
            if (authorized)
            {
                if (string.Equals(entity.OrganisationId, existingEntity.OrganisationId))
                {
                    var result = await amphoraStore.UpdateAsync(entity);
                    return new EntityOperationResult<Common.Models.Amphora>(result);
                }
                else
                {
                    return new EntityOperationResult<Common.Models.Amphora>("Cannot change organisation id");
                }
            }
            else
            {
                return new EntityOperationResult<Common.Models.Amphora>("Unauthorized") { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<Common.Models.Amphora>> DeleteAsync(ClaimsPrincipal principal, Common.Models.Amphora entity)
        {
            logger.LogInformation($"Deleting Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, entity.OrganisationId);
            if (existingEntity == null)
            {
                logger.LogError($"{entity.Id} Not Found");
                return new EntityOperationResult<Common.Models.Amphora>($"{entity.Id} Not Found");
            }
            var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Delete);
            if (authorized)
            {
                await amphoraStore.DeleteAsync(entity);
                return new EntityOperationResult<Common.Models.Amphora>();
            }
            else
            {
                return new EntityOperationResult<Common.Models.Amphora>("Unauthorized") { WasForbidden = true };
            }
        }

        private async Task<(IApplicationUser user, Common.Models.Amphora entity)> GetUserAndEntityAsync(ClaimsPrincipal principal, string id, string orgId)
        {
            id = id.AsQualifiedId<Common.Models.Amphora>();
            var user = await userManager.GetUserAsync(principal);
            var entity = await amphoraStore.ReadAsync(id, orgId);
            return (user, entity);
        }
    }
}