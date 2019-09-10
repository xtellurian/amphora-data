using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Amphora.Common.Extensions;
using static Amphora.Common.Models.RoleAssignment;
using System;

namespace Amphora.Api.Services.Auth
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
        public async Task<bool> IsAuthorizedAsync(IApplicationUser user, IEntity entity, string resourcePermission)
        {
            // var collection = await permissionStore.QueryAsync(c => string.Equals(c.OrganisationId, entity.OrganisationId));
            var collection = await permissionStore.ReadAsync(
                entity.OrganisationId.AsQualifiedId(typeof(PermissionCollection)),
                entity.OrganisationId);
            if(collection == null) 
            {
                logger.LogWarning($"{entity.OrganisationId.AsQualifiedId(typeof(PermissionCollection))} not found");
                return false;
            }
            // check if user is in Admin role
            var usersAssignment = collection.RoleAssignments.FirstOrDefault(r => string.Equals(r.UserId, user.Id));
            if (usersAssignment != null)
            {
                if (usersAssignment.Role == Roles.Administrator)
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Admin for entity {entity.Id}");
                    return true;
                }
                else if(usersAssignment.Role == Roles.User && string.Equals(resourcePermission, ResourcePermissions.Read))
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Org User for entity {entity.Id}");
                    return true;
                }
                else if(usersAssignment.Role == Roles.User && string.Equals(resourcePermission, ResourcePermissions.ReadContents))
                {
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as Org User for entity {entity.Id}");
                    return true;
                }
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
        public async Task<PermissionCollection> SetIsOwner(IApplicationUser user, IEntity entity)
        {
            var collection = await CreateIfNotExistsCollection(entity);

            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Read));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Update));
            collection.ResourceAuthorizations.Add(new ResourceAuthorization(user.Id, entity, ResourcePermissions.Delete));

            return await permissionStore.UpdateAsync(collection);
        }

        public async Task<PermissionCollection> CreateOrganisationalRole(IApplicationUser user, Roles role, Organisation org)
        {
            var assignment = new RoleAssignment(user.Id, role);
            var collection = await CreateIfNotExistsCollection(org);
            collection.RoleAssignments.Add(assignment);
            return await permissionStore.UpdateAsync(collection);
        }

        private async Task<PermissionCollection> CreateIfNotExistsCollection(IEntity entity)
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
            return collection;
        }
    }
}