using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Permissions.Rules;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IEntityStore<AmphoraModel> amphoraeStore;
        private readonly IUserDataService userDataService;

        public PermissionService(IEntityStore<OrganisationModel> orgStore,
                                IEntityStore<AmphoraModel> amphoraeStore,
                                IUserDataService userDataService,
                                ILogger<PermissionService> logger)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.amphoraeStore = amphoraeStore;
            this.userDataService = userDataService;
        }

        public async Task<bool> IsAuthorizedAsync(ClaimsPrincipal principal, AmphoraModel entity, AccessLevels accessLevel)
        {
            var userDataRes = await userDataService.ReadAsync(principal);
            return userDataRes.Succeeded && await this.IsAuthorizedAsync(userDataRes.Entity, entity, accessLevel);
        }

        public async Task<bool> IsAuthorizedAsync(IUser user, OrganisationModel org, AccessLevels accessLevel)
        {
            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                if (org == null) { return false; }
                org = await orgStore.ReadAsync(org.Id);
                if (accessLevel <= AccessLevels.Read)
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
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            using (logger.BeginScope(new LoggerScope<PermissionService>(user)))
            {
                // check if user is in Admin role
                var org = await orgStore.ReadAsync(entity.OrganisationId);
                if (org == null)
                {
                    logger.LogError($"null Organisation for amohora {entity.Id}");
                    return false;
                }

                if (user.OrganisationId == entity.OrganisationId
                    && accessLevel <= AccessLevels.CreateEntities)
                {
                    // everyone in an org should be able to create an Amphora
                    return true;
                }

                if (user.OrganisationId == entity.OrganisationId
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

                if (AccessModelApplies(user, entity, accessLevel))
                {
                    // then we must ruturn here one way or the other
                    logger.LogInformation($"{user.UserName} is accessing via Access Controls Amphora({entity.Id})");
                    return IsAllowed(user, entity, accessLevel);
                }

                if (IsAccessDenied(user, entity, accessLevel))
                {
                    logger.LogInformation($"{user.UserName} is restricted on Amphora {entity.Id}");
                    return false;
                }

                var usersOrganisation = await orgStore.ReadAsync(user.OrganisationId);
                if (accessLevel <= AccessLevels.ReadContents && HasOrganisationPurchasedAmphora(usersOrganisation, entity))
                {
                    logger.LogInformation($"Authorization granted for user {user.Id} for amphora {entity.Id} - has purchased");
                    return true;
                }

                if (accessLevel <= AccessLevels.Purchase && entity.Public())
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

        private bool IsAllowed(IUser user, AmphoraModel amphora, AccessLevels accessLevel)
        {
            // get all the rules that apply to the user
            var rules = amphora.AccessControl?.Rules();
            if (accessLevel > AccessLevels.ReadContents)
            {
                logger.LogInformation($"User({user.UserName}) was denied access to Amphora({amphora.Id}) because the access level was too high");
                return false;
            }

            if (rules != null && rules.Any())
            {
                var applicableRules = new List<AccessRule>();
                if (amphora.AccessControl.AllRule != null)
                {
                    // always add the all rule if it exists.
                    applicableRules.Add(amphora.AccessControl.AllRule);
                }

                var userRules = amphora.AccessControl.UserAccessRules.Where(_ => _.UserDataId == user.Id);
                var orgRules = amphora.AccessControl.OrganisationAccessRules.Where(_ => _.OrganisationId == user.OrganisationId);
                applicableRules.AddRange(orgRules);
                applicableRules.AddRange(userRules);
                var maxPriority = applicableRules.Max(_ => _.Priority);
                var ruleToApply = applicableRules.FirstOrDefault(_ => _.Priority == maxPriority);
                var allowed = ruleToApply?.Kind == Kind.Allow;
                logger.LogInformation($"User({user.UserName}) access granted: {allowed}");
                return allowed;
            }
            else
            {
                return false;
            }
        }

        private bool AccessModelApplies(IUser user, AmphoraModel amphora, AccessLevels accessLevel)
        {
            if (accessLevel <= AccessLevels.Read || accessLevel > AccessLevels.ReadContents)
            {
                // access model only applied to reading the contents
                return false;
            }

            if (amphora?.AccessControl?.AllRule != null)
            {
                return true;
            }

            var rules = amphora.AccessControl?.Rules();
            if (rules != null && rules.Any())
            {
                if (amphora.AccessControl.UserAccessRules.Any(_ => _.UserDataId == user.Id))
                {
                    return true;
                }
                else if (amphora.AccessControl.OrganisationAccessRules.Any(_ => _.OrganisationId == user.OrganisationId))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAccessDenied(IUser user, AmphoraModel amphora, AccessLevels accessLevel)
        {
            if (accessLevel >= AccessLevels.Purchase)
            {
                if (amphora.AccessControl == null)
                {
                    return false;
                }

                var isUserDenied = amphora.AccessControl.UserAccessRules
                    .Any(_ => _.UserDataId == user.Id && _.Kind == Kind.Deny);
                var isOrgDenied = amphora.AccessControl.OrganisationAccessRules
                    .Any(_ => _.OrganisationId == user.OrganisationId && _.Kind == Kind.Deny);

                return isUserDenied || isOrgDenied;
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

        private bool HasOrganisationPurchasedAmphora(OrganisationModel organisation, AmphoraModel amphora)
        {
            var hasPurchased = amphora.Purchases?.Any(p => string.Equals(p.PurchasedByOrganisationId, organisation.Id)) ?? false;
            logger.LogInformation($"Has Purchased: {hasPurchased}");
            return hasPurchased;
        }
    }
}