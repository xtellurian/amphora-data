using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService : IAmphoraeService
    {
        public IEntityStore<AmphoraModel> AmphoraStore { get; private set; }

        private readonly IOptionsMonitor<AmphoraManagementOptions> options;
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IEntityStore<OrganisationModel> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;
        private readonly IEventPublisher eventPublisher;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IOptionsMonitor<AmphoraManagementOptions> options,
                               IEntityStore<AmphoraModel> amphoraStore,
                               IEntityStore<PurchaseModel> purchaseStore,
                               IEntityStore<OrganisationModel> organisationStore,
                               IPermissionService permissionService,
                               IUserDataService userDataService,
                               IEventPublisher eventPublisher,
                               ILogger<AmphoraeService> logger)
        {
            this.options = options;
            AmphoraStore = amphoraStore;
            this.purchaseStore = purchaseStore;
            this.organisationStore = organisationStore;
            this.permissionService = permissionService;
            this.userDataService = userDataService;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraModel>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                // set the required fields
                if (string.IsNullOrEmpty(model.OrganisationId)) { model.OrganisationId = userReadRes.Entity.OrganisationId; }
                model.CreatedById = userReadRes.Entity.Id;
                model.CreatedBy = userReadRes.Entity;
                model.CreatedDate = DateTime.UtcNow;
                model.LastModified = DateTime.UtcNow;

                var organisation = await organisationStore.ReadAsync(model.OrganisationId);

                if (string.IsNullOrEmpty(model.TermsAndConditionsId))
                {
                    var tnc = organisation.TermsAndConditions?.FirstOrDefault();
                    logger.LogInformation($"Using default terms and conditions: {tnc?.Id}");
                    model.TermsAndConditionsId = tnc?.Id;
                }
                else
                {
                    // check tnc's exist
                    var tnc = organisation.TermsAndConditions.FirstOrDefault(_ => _.Id == model.TermsAndConditionsId);
                    if (tnc == null)
                    {
                        logger.LogInformation($"CreateAsync failed with invalid TermsAndConditionsId: {model.TermsAndConditionsId}");
                        var allTermsAndConditions = string.Join(',', organisation.TermsAndConditions.Select(_ => _.Id));
                        return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, $"Invalid TermsAndConditionsId. Use one of {allTermsAndConditions}");
                    }
                }

                // check permission to create amphora
                logger.LogInformation($"Creating new Amphora for OrganisationId {organisation.Id}");
                var isAuthorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, organisation, ResourcePermissions.Create);
                if (isAuthorized)
                {
                    model = await AmphoraStore.CreateAsync(model);
                    await eventPublisher.PublishEventAsync(new AmphoraCreatedEvent(model));
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, model);
                }
                else
                {
                    logger.LogWarning("User Unauthorized");
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, "Unauthorized", $"Create permission required on {organisation.Id}")
                    { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> ReadAsync(ClaimsPrincipal principal,
                                                                         string id,
                                                                         bool includeChildren = false,
                                                                         string orgId = null)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraModel>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                var entity = await AmphoraStore.ReadAsync(id);
                logger.LogInformation($"Reading Amphora {id}");

                if (entity == null)
                {
                    logger.LogError($"{id} Not Found");
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, $"{id} Not Found");
                }

                if (entity.Public())
                {
                    logger.LogInformation($"Permission granted to public entity {entity.Id}");
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, entity);
                }

                var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.Read);
                if (authorized)
                {
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, entity);
                }
                else
                {
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, "Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraModel>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                var existingEntity = await AmphoraStore.ReadAsync(entity.Id);
                logger.LogInformation($"Updating Amphora {entity.Id}");

                if (existingEntity == null)
                {
                    logger.LogError($"{entity.Id} Not Found");
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, $"{entity.Id} Not Found");
                }

                var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.Update);
                if (authorized)
                {
                    if (string.Equals(entity.OrganisationId, existingEntity.OrganisationId))
                    {
                        var result = await AmphoraStore.UpdateAsync(entity);
                        return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, result);
                    }
                    else
                    {
                        return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, "Cannot change organisation id");
                    }
                }
                else
                {
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, "Unauthorized") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            if (entity == null) { throw new NullReferenceException("Entity cannot be null"); }
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraModel>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(userReadRes.Entity)))
            {
                logger.LogInformation($"Deleting Amphora {entity.Id}");
                var existingEntity = await AmphoraStore.ReadAsync(entity.Id);
                if (existingEntity == null)
                {
                    logger.LogError($"{entity.Id} Not Found");
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, $"{entity.Id} Not Found");
                }

                var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.Delete);
                if (authorized)
                {
                    if (options.CurrentValue?.SoftDelete.Value ?? false)
                    {
                        entity.IsDeleted = true;
                        entity.ttl = options.CurrentValue?.DeletedTimeToLive ?? 48 * 60 * 60;
                        await AmphoraStore.UpdateAsync(entity);
                    }
                    else
                    {
                        await AmphoraStore.DeleteAsync(entity);
                    }

                    return new EntityOperationResult<AmphoraModel>(true);
                }
                else
                {
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, "Unauthorized") { WasForbidden = true };
                }
            }
        }
    }
}