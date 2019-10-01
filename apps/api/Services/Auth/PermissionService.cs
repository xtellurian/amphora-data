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
using Amphora.Common.Models.Users;

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

        public async Task<bool> IsAuthorizedAsync(IUser user, OrganisationModel org, AccessLevels accessLevel)
        {
            org = await orgStore.ReadAsync(org.Id, true);
            var membership = org.Memberships?.FirstOrDefault(m => string.Equals(m.UserModelId, user.Id));

            if (membership != null) // if user is in the org
            {
                if (membership.Role.ToDefaultAccessLevel() >= accessLevel)
                {
                    // permission granted via role
                    logger.LogInformation($"Authorization succeeded for user {user.Id} as role {Enum.GetName(typeof(AccessLevels), membership.Role)} for org {org.Id}");
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> IsAuthorizedAsync(IUser user, AmphoraModel entity, AccessLevels accessLevel)
        {
            // check if user is in Admin role
            var org = await orgStore.ReadAsync(entity.OrganisationId);
            if(await this.IsAuthorizedAsync(user, org, accessLevel))
            {
                return true;
            }

            if( await HasUserPurchasedAmphoraAsync(user, entity as AmphoraModel) && accessLevel <= AccessLevels.ReadContents)
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

        private async Task<bool> HasUserPurchasedAmphoraAsync(IUser user, AmphoraModel amphora)
        {
            var extended = await amphoraeStore.ReadAsync(amphora.Id);
            return extended.Transactions?.Any(p => string.Equals(p.UserId , user.Id)) ?? false;
        }
    }
}