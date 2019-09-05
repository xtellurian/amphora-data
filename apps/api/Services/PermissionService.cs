using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Amphora.Common.Extensions;

namespace Amphora.Api.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> logger;
        private readonly IEntityStore<PermissionCollection> permissionStore;

        public PermissionService(ILogger<PermissionService> logger,
                                 IEntityStore<PermissionCollection> permissionStore)
        {
            this.logger = logger;
            this.permissionStore = permissionStore;
        }
        public async Task<bool> IsAuthorized(IApplicationUser user, IEntity entity, string resourcePermission)
        {
            var collection = await permissionStore.QueryAsync(c => string.Equals(c.OrganisationId, entity.OrganisationId));
            bool succeeded = false;
            foreach (var c in collection)
            {
                var auth = c.ResourceAuthorizations.FirstOrDefault(p =>
                    string.Equals(p.ResourcePermission, resourcePermission)
                    && string.Equals(p.TargetResourceId, entity.Id)
                    && string.Equals(p.UserId, user.Id)
                );
                if (auth != null)
                {
                    succeeded = true;
                    logger.LogInformation($"Authorization succeeded for user {user.Id} to entity {entity.Id}");
                    break;
                }
            }

            return succeeded;
        }

        public async Task<PermissionCollection> SetIsOwner(IApplicationUser user, IEntity entity)
        {
            var collection = await permissionStore.ReadAsync(
                entity.OrganisationId.AsQualifiedId(typeof(PermissionCollection)),
                entity.OrganisationId
            );
            if (collection == null)
            {
                collection = new PermissionCollection(entity.OrganisationId);
                collection = await permissionStore.CreateAsync(collection);
            }

            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Read));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Update));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Delete));

            return await permissionStore.UpdateAsync(collection);
        }
    }
}