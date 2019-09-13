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
using System.Collections.Generic;
using Amphora.Common.Models.Permissions;

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
        public async Task<bool> IsAuthorizedAsync(IApplicationUser user, IEntity entity, AccessLevels accessLevel)
        {
            // check if user is in Admin role
            var org = await orgStore.ReadAsync<OrganisationExtendedModel>(entity.OrganisationId, entity.OrganisationId);
            var membership = org.Memberships?.FirstOrDefault(m => string.Equals(m.UserId, user.Id));

            if (membership != null) // if user is in the org
            {
                if (membership.Role.ToDefaultAccessLevel() >= accessLevel)
                {
                    // permission granted via role
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as role {Enum.GetName(typeof(AccessLevels), membership.Role)} for entity {entity.Id}");
                    return true;
                }
            }
            var authorizationForUser = await this.ReadUserAuthorizationAsync(entity, user);
            if (authorizationForUser == null) return false;

            // check if CRUD permission exists
            if (authorizationForUser.AccessLevel > accessLevel)
            {
                logger.LogInformation($"Authorization succeeded for user {user.Id} for entity {entity.Id}");
                return true;
            }

            logger.LogInformation($"Authorization denied for user {user.Id} for entity {entity.Id}");
            return false;
        }

        public async Task<IEnumerable<ResourceAuthorization>> ListAuthorizationsAsync(IEntity entity)
        {
            PermissionModel permissions = await ReadModelAsync(entity);
            if (permissions == null) logger.LogWarning($"{entity?.OrganisationId.AsQualifiedId(typeof(PermissionModel))} permissions not found");


            return permissions?.ResourceAuthorizations.Where(p =>
                    string.Equals(p.TargetEntityId, entity?.Id)
                );
        }

        private async Task<PermissionModel> ReadModelAsync(IEntity entity)
        {
            var permission = await permissionStore.ReadAsync(
                            entity?.OrganisationId.AsQualifiedId(typeof(PermissionModel)),
                            entity?.OrganisationId);
            if(permission == null)
            {
                logger.LogWarning($"Creating new permission model for org {entity.OrganisationId}");
                permission = new PermissionModel(entity.OrganisationId);
                permission = await permissionStore.CreateAsync(permission);
            }
            return permission;
        }

        public async Task<ResourceAuthorization> ReadUserAuthorizationAsync(IEntity entity, IApplicationUser user)
        {
            var all = await this.ListAuthorizationsAsync(entity);
            if (all == null) return null;
            if (all.Count(u => string.Equals(u.UserId, user.Id)) > 1)
            {
                logger.LogWarning($"Multiple authorizations for {user.Id} on {entity.Id}");
            }
            return all.FirstOrDefault(u => string.Equals(u.UserId, user.Id));
        }

        public async Task UpdatePermissionAsync(IApplicationUser user, IEntity entity, AccessLevels level)
        {
            if(user == null)
            {
                throw new NullReferenceException("User was null");
            }
            var model = await ReadModelAsync(entity);
            var authorizations = model.GetAuthorizations(entity);
            var auth = authorizations.FirstOrDefault(a => string.Equals(a.UserId, user.Id));
            if(auth == null)
            {
                auth = new ResourceAuthorization(user.Id, user.UserName, entity, AccessLevels.None );
                model.ResourceAuthorizations.Add(auth);
            }

            auth.AccessLevel = level;
            await permissionStore.UpdateAsync(model);
        }
    }
}