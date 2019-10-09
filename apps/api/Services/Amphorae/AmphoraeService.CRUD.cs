using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using System.Collections.Generic;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService : IAmphoraeService
    {
        public IEntityStore<AmphoraModel> AmphoraStore { get; private set; }

        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IEntityStore<OrganisationModel> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IEntityStore<AmphoraModel> amphoraStore,
                               IEntityStore<PurchaseModel> purchaseStore,
                               IEntityStore<OrganisationModel> organisationStore,
                               IPermissionService permissionService,
                               IUserService userService,
                               ILogger<AmphoraeService> logger)
        {
            AmphoraStore = amphoraStore;
            this.purchaseStore = purchaseStore;
            this.organisationStore = organisationStore;
            this.permissionService = permissionService;
            this.userService = userService;
            this.logger = logger;
        }
        public async Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(user)))
            {
                // set the required fields
                if (string.IsNullOrEmpty(model.OrganisationId)) model.OrganisationId = user.OrganisationId;
                model.CreatedById = user.Id;
                model.CreatedBy = user;
                model.CreatedDate = DateTime.UtcNow;

                // check permission to create amphora
                var organisation = await organisationStore.ReadAsync(
                    model.OrganisationId);
                logger.LogInformation($"Creating new Amphora for OrganisationId {organisation.Id}");
                var isAuthorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Create);
                if (isAuthorized)
                {
                    model = await AmphoraStore.CreateAsync(model);
                    return new EntityOperationResult<AmphoraModel>(model);
                }
                else
                {
                    logger.LogWarning("User Unauthorized");
                    return new EntityOperationResult<AmphoraModel>("Unauthorized", $"Create permission required on {organisation.Id}")
                    { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> ReadAsync(ClaimsPrincipal principal, string id, bool includeChildren = false, string orgId = null)
        {
            var user = await userService.ReadUserModelAsync(principal);

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(user)))
            {
                var entity = await AmphoraStore.ReadAsync(id, includeChildren);
                logger.LogInformation($"Reading Amphora {id}");

                if (entity == null)
                {
                    logger.LogError($"{id} Not Found");
                    return new EntityOperationResult<AmphoraModel>($"{id} Not Found");
                }
                if (entity.IsPublic)
                {
                    logger.LogInformation($"Permission granted to public entity {entity.Id}");
                    return new EntityOperationResult<AmphoraModel>(entity);
                }

                var authorized = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.Read);
                if (authorized)
                {
                    return new EntityOperationResult<AmphoraModel>(entity);
                }
                else
                {
                    return new EntityOperationResult<AmphoraModel>("Denied") { WasForbidden = true };
                }
            }

        }

        public async Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(user)))
            {
                var existingEntity = await AmphoraStore.ReadAsync(entity.Id, false);
                logger.LogInformation($"Updating Amphora {entity.Id}");

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
        }

        public async Task<EntityOperationResult<AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            if (entity == null) throw new NullReferenceException("Entity cannot be null");
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraeService>(user)))
            {
                logger.LogInformation($"Deleting Amphora {entity.Id}");
                var existingEntity = await AmphoraStore.ReadAsync(entity.Id, false);
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
        }
    }
}