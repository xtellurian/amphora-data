using System;
using System.Collections.Generic;
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
        private readonly ITermsOfUseService termsOfUseService;
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;
        private readonly IEventRoot eventPublisher;
        private readonly ILogger<AmphoraeService> logger;

        public AmphoraeService(IOptionsMonitor<AmphoraManagementOptions> options,
                               IEntityStore<AmphoraModel> amphoraStore,
                               IEntityStore<PurchaseModel> purchaseStore,
                               IEntityStore<OrganisationModel> organisationStore,
                               ITermsOfUseService termsOfUseService,
                               IPermissionService permissionService,
                               IUserDataService userDataService,
                               IEventRoot eventPublisher,
                               ILogger<AmphoraeService> logger)
        {
            this.options = options;
            AmphoraStore = amphoraStore;
            this.purchaseStore = purchaseStore;
            this.organisationStore = organisationStore;
            this.termsOfUseService = termsOfUseService;
            this.permissionService = permissionService;
            this.userDataService = userDataService;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        private (bool isValid, string message) ArePropertiesValid(AmphoraModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return (false, "Name is null or empty");
            }
            else if (model.Name?.Length > 120)
            {
                return (false, "Name is longer than 120 chars");
            }
            else
            {
                // true unless false;
                return (true, null);
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraModel>("Unknown User");
            }

            (var isValid, var message) = ArePropertiesValid(model);
            if (!isValid)
            {
                return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, message);
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

                if (string.IsNullOrEmpty(model.TermsOfUseId))
                {
                    logger.LogInformation($"Using no terms of use.");
                    model.TermsOfUseId = null;
                }
                else
                {
                    // check tnc's exist
                    var touReadRes = await termsOfUseService.ReadAsync(principal, model.TermsOfUseId);
                    if (!touReadRes.Succeeded)
                    {
                        return new EntityOperationResult<AmphoraModel>("Unknown terms of use");
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

        public async Task<EntityOperationResult<IEnumerable<AmphoraModel>>> ListForSelfAsync(ClaimsPrincipal principal,
                                                                                             int skip = 0,
                                                                                             int take = 64,
                                                                                             bool owned = true,
                                                                                             bool purchased = false)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<IEnumerable<AmphoraModel>>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                var res = new List<AmphoraModel>();
                if (owned)
                {
                    res.AddRange(await AmphoraStore.QueryAsync(_ => _.CreatedById == userReadRes.Entity.Id, skip, take));
                }

                if (purchased)
                {
                    // .Select can't be translated into cosmos
                    var purchases = await purchaseStore.QueryAsync(_ => _.PurchasedByUserId == userReadRes.Entity.Id, skip, take);
                    res.AddRange(purchases.Select(_ => _.Amphora));
                }

                return new EntityOperationResult<IEnumerable<AmphoraModel>>(userReadRes.Entity, res);
            }
        }

        public async Task<EntityOperationResult<IEnumerable<AmphoraModel>>> ListForOrganisationAsync(ClaimsPrincipal principal,
                                                                                                     int skip = 0,
                                                                                                     int take = 64,
                                                                                                     bool created = true,
                                                                                                     bool purchased = false)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<IEnumerable<AmphoraModel>>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraeService>(principal)))
            {
                var res = new List<AmphoraModel>();
                if (created)
                {
                    res.AddRange(await AmphoraStore.QueryAsync(_ => _.OrganisationId == userReadRes.Entity.OrganisationId, skip, take));
                }

                if (purchased)
                {
                    // .Select can't be translated into cosmos
                    var purchases = await purchaseStore.QueryAsync(_ => _.PurchasedByOrganisationId == userReadRes.Entity.OrganisationId, skip, take);
                    res.AddRange(purchases.Select(_ => _.Amphora));
                }

                return new EntityOperationResult<IEnumerable<AmphoraModel>>(userReadRes.Entity, res);
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
                    return new EntityOperationResult<AmphoraModel>(userReadRes.Entity, $"{id} Not Found") { Code = 404 };
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