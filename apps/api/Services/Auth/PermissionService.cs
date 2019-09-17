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
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IEntityStore<AmphoraModel> amphoraeStore;

        public PermissionService(ILogger<PermissionService> logger,
                                IEntityStore<OrganisationModel> orgStore,
                                IEntityStore<AmphoraModel> amphoraeStore)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.amphoraeStore = amphoraeStore;
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

            if(entity is AmphoraModel && await HasUserPurchasedAmphoraAsync(user, entity as AmphoraModel) && accessLevel <= AccessLevels.ReadContents)
            {
                logger.LogInformation($"Authorization granted for user {user.Id} for amphora {entity.Id} - has purchased");
                return true;
            }
            else
            {
                logger.LogInformation($"Authorization denied for user {user.Id} for {entity.Id}");
                return false;
            }
        }

        private async Task<bool> HasUserPurchasedAmphoraAsync(IApplicationUserReference user, AmphoraModel amphora)
        {
            var extended = await amphoraeStore.ReadAsync<AmphoraSecurityModel>(amphora.Id, amphora.OrganisationId);
            return extended.HasPurchased?.Any(p => string.Equals(p.Id , user.Id)) ?? false;
        }
    }
}