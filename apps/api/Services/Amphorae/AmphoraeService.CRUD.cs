using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Transactions;
using Amphora.Common.Models.Users;

namespace Amphora.Api.Services.Amphorae
{
    public partial class AmphoraeService : IAmphoraeService
    {
        private readonly IEntityStore<TransactionModel> transactionStore;
        private readonly IEntityStore<OrganisationModel> organisationStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly ILogger<AmphoraeService> logger;
        public IEntityStore<AmphoraModel> AmphoraStore { get; }

        public AmphoraeService(IEntityStore<AmphoraModel> amphoraStore,
                               IEntityStore<TransactionModel> transactionStore,
                               IEntityStore<OrganisationModel> organisationStore,
                               IPermissionService permissionService,
                               IUserService userService,
                               ILogger<AmphoraeService> logger)
        {
            AmphoraStore = amphoraStore;
            this.transactionStore = transactionStore;
            this.organisationStore = organisationStore;
            this.permissionService = permissionService;
            this.userService = userService;
            this.logger = logger;
        }
        public async Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model)
        {
            logger.LogInformation($"Creating new Amphora");
            var user = await userService.ReadUserModelAsync(principal);
            // set the required fields
            if (string.IsNullOrEmpty(model.OrganisationId)) model.OrganisationId = user.OrganisationId;
            model.CreatedBy = user.Id;
            model.CreatedDate = DateTime.UtcNow;

            if (!model.IsValid()) throw new NullReferenceException("Invalid Amphora Model");
            
            // check permission to create amphora
            var organisation = await organisationStore.ReadAsync(
                model.OrganisationId);

            var isAuthorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Create);
            if (isAuthorized)
            {
                model.Organisation = organisation;
                model = await AmphoraStore.CreateAsync(model);
                // await searchService.Reindex();
                return new EntityOperationResult<AmphoraModel>(model);
            }
            else
            {
                return new EntityOperationResult<AmphoraModel>("Unauthorized", $"Create permission required on {organisation.Id}")
                { WasForbidden = true };
            }
        }

        public async Task<EntityOperationResult<AmphoraModel>> ReadAsync(ClaimsPrincipal principal, string id, bool includeChildren = false, string orgId = null)
        {
            logger.LogDebug($"Reading Amphora {id}");
            var (user, entity) = await GetUserAndEntityAsync(principal, id, includeChildren, orgId);
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

        public async Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            logger.LogDebug($"Updating Amphora {entity.Id}");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, false, entity.OrganisationId);
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
            if(entity == null) throw new NullReferenceException("Entity cannot be null");
            var (user, existingEntity) = await GetUserAndEntityAsync(principal, entity.Id, false, entity.OrganisationId);
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

        private async Task<(ApplicationUser user, AmphoraModel entity)> GetUserAndEntityAsync(ClaimsPrincipal principal, string id, bool includeChildren, string orgId) 
        {
            var user = await userService.ReadUserModelAsync(principal);
            var entity = await AmphoraStore.ReadAsync(id, includeChildren);
            return (user, entity);
        }
    }
}