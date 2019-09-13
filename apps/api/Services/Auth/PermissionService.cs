using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Amphora.Common.Extensions;
using System;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Services.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IEntityStore<PermissionModel> permissionStore;

        public PermissionService(ILogger<PermissionService> logger,
                                IEntityStore<OrganisationModel> orgStore,
                                 IEntityStore<PermissionModel> permissionStore)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.permissionStore = permissionStore;
        }
        public async Task<bool> IsAuthorizedAsync(IApplicationUser user, IEntity entity, string resourcePermission)
        {
            // check if user is in Admin role
            var org = await orgStore.ReadAsync<OrganisationExtendedModel>(entity.OrganisationId, entity.OrganisationId);
            var membership = org.Memberships?.FirstOrDefault(m => string.Equals(m.UserId, user.Id));
            if (membership != null) // if user is in the org
            {
                if (membership.Role == Roles.Administrator)
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Admin for entity {entity.Id}");
                    return true;
                }
                else if (membership.Role == Roles.User && string.Equals(resourcePermission, ResourcePermissions.Read))
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Org User for entity {entity.Id}");
                    return true;
                }
                else if (membership.Role == Roles.User && string.Equals(resourcePermission, ResourcePermissions.ReadContents))
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Org User for entity {entity.Id}");
                    return true;
                }
            }

            var collection = await permissionStore.ReadAsync(
                entity.OrganisationId.AsQualifiedId(typeof(PermissionModel)),
                entity.OrganisationId);
            if (collection == null)
            {
                logger.LogWarning($"{entity.OrganisationId.AsQualifiedId(typeof(PermissionModel))} not found");
                return false;
            }

            // check if CRUD permission exists
            if (collection?.ResourceAuthorizations != null)
            {
                var auth = collection.ResourceAuthorizations.FirstOrDefault(p =>
                    string.Equals(p.ResourcePermission, resourcePermission)
                    && string.Equals(p.TargetResourceId, entity.Id)
                    && string.Equals(p.UserId, user.Id)
                );
                if (auth != null)
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} for entity {entity.Id}");
                    return true;
                }
            }
            logger.LogInformation($"Authorization denied for user {user.Id} for entity {entity.Id}");
            return false;
        }

        [Obsolete]
        public async Task<PermissionModel> SetIsOwner(IApplicationUser user, IEntity entity)
        {
            var collection = await CreateIfNotExistsCollection(entity);

            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Read));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Update));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Delete));

            return await permissionStore.UpdateAsync(collection);
        }

        private async Task<PermissionModel> CreateIfNotExistsCollection(IEntity entity)
        {
            var collection = await permissionStore.ReadAsync(
                            entity.OrganisationId.AsQualifiedId(typeof(PermissionModel)),
                            entity.OrganisationId
                        );
            if (collection == null)
            {
                collection = new PermissionModel(entity.OrganisationId);
                collection = await permissionStore.CreateAsync(collection);
            }
            return collection;
        }
    }
}