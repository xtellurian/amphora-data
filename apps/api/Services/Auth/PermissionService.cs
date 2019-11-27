using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using Amphora.Common.Models.Organisations;
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

        public PermissionService(IEntityStore<OrganisationModel> orgStore,
                                IEntityStore<AmphoraModel> amphoraeStore,
                                ILogger<PermissionService> logger)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.amphoraeStore = amphoraeStore;
        }

        public async Task<bool> IsAuthorizedAsync(IUser user, OrganisationModel org, AccessLevels accessLevel)
        {
            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                if (org == null) return false;
                org = await orgStore.ReadAsync(org.Id);
                if(accessLevel <= AccessLevels.Read) 
                {
                    logger.LogInformation($"Default read for org {org.Id}");
                    return true;
                }
                var membership = org.Memberships?.FirstOrDefault(m => string.Equals(m.UserId, user.Id));

                if (membership != null) // if user is in the org
                {
                    if (membership.Role.ToDefaultAccessLevel() >= accessLevel)
                    {
                        // permission granted via role
                        logger.LogInformation($"Authorization succeeded as role {Enum.GetName(typeof(AccessLevels), membership.Role)} for org {org.Id}");
                        return true;
                    }
                }
                logger.LogInformation($"Authorization failed for org {org.Id}");
                return false;
            }
        }
        public async Task<bool> IsAuthorizedAsync(IUser user, AmphoraModel entity, AccessLevels accessLevel)
        {
            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                // check if user is in Admin role
                var org = await orgStore.ReadAsync(entity.OrganisationId);
                if(org == null)
                {
                    logger.LogError($"null Organisation for amohora {entity.Id}");
                    return false;
                }
                if(user.OrganisationId == entity.OrganisationId 
                    && accessLevel <= AccessLevels.CreateAmphora )
                {
                    // everyone in an org should be able to create an Amphora
                    return true;                     
                }
                if(user.OrganisationId == entity.OrganisationId 
                    && user.Id == entity.CreatedById 
                    && accessLevel <= AccessLevels.Update)
                {
                    
                    // can always update the Amphora you created.
                    return true;
                }
                if (await this.IsAuthorizedAsync(user, org, accessLevel))
                {
                    return true;
                }
                if (IsRestricted(user, org, accessLevel))
                {
                    logger.LogInformation($"{user.UserName} is restricted on Amphora {entity.Id}");
                    return false;
                }
                if (HasUserPurchasedAmphora(user, entity) && accessLevel <= AccessLevels.ReadContents)
                {
                    logger.LogInformation($"Authorization granted for user {user.Id} for amphora {entity.Id} - has purchased");
                    return true;
                }
                if(accessLevel <= AccessLevels.Purchase && entity.Public())
                {
                    logger.LogInformation($"Default authorize level {accessLevel.ToString()} for user {user.Id} for {entity.Id}");
                    return true;
                }
                else
                {
                    logger.LogInformation($"Authorization denied for user {user.Id} for {entity.Id}");
                    return false;
                }
            }
        }

        private bool IsRestricted(IUser user, OrganisationModel organisation, AccessLevels accessLevel)
        {
            if (accessLevel >= AccessLevels.Purchase)
            {
                return organisation.Restrictions.Any(_ => _.TargetOrganisationId == user.OrganisationId && _.Kind == RestrictionKind.Deny);
            }
            else
            {
                return false;
            }
        }

        private bool HasUserPurchasedAmphora(IUser user, AmphoraModel amphora)
        {
            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                var hasPurchased = amphora.Purchases?.Any(p => string.Equals(p.PurchasedByUserId, user.Id)) ?? false;
                logger.LogInformation($"Has Purchased: {hasPurchased}");
                return hasPurchased;
            }
        }
    }
}