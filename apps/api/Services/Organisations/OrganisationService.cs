using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Extensions;
using Amphora.Common.Exceptions;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Amphora.Api.Services.Organisations
{
    // step 1: create user
    // step 2: create Org and asign user to that org
    public class OrganisationService : IOrganisationService
    {
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        public IEntityStore<OrganisationModel> Store { get; }
        private readonly ILogger<OrganisationService> logger;

        public OrganisationService(
            IUserService userService,
            IPermissionService permissionService,
            IEntityStore<OrganisationModel> orgStore,
            ILogger<OrganisationService> logger)
        {
            this.userService = userService;
            this.permissionService = permissionService;
            this.Store = orgStore;
            this.logger = logger;
        }

        public async Task<bool> AcceptInvitation(ClaimsPrincipal principal, string orgId)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            var org = await Store.ReadAsync<OrganisationExtendedModel>(orgId, orgId);
            var invitation = org.Invitations?.FirstOrDefault(i => string.Equals(i.TargetEmail.ToUpper(), user.Email.ToUpper()));

            if(invitation != null)
            {
                user.OrganisationId = org.OrganisationId;
                var result = await userService.UserManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    logger.LogInformation($"{user.Email} redeemed an invitation to {org.Id}");
                    org.Invitations.Remove(invitation);
                    org.AddOrUpdateMembership(user);
                    await Store.UpdateAsync(org);
                    return true;
                }
                else
                {
                    logger.LogError($"{user.Id} failed to redeem invidation to {org.Id} ");
                    return false;
                }
            }
            else
            {
                logger.LogError($"{user.Id} tried to access {org.Id} without invitation");
                return false;
            }
        }

        public async Task InviteToOrganisationAsync(ClaimsPrincipal principal, string orgId, string email)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            var org = await Store.ReadAsync<OrganisationExtendedModel>(orgId, orgId);
            var authorized = await permissionService.IsAuthorizedAsync(user, org, ResourcePermissions.Create);
            if (authorized)
            {
                var invitation = new Invitation(email);
                org.AddInvitation(email);
                var result = await Store.UpdateAsync(org);
            }
            else
            {
                throw new PermissionDeniedException($"{user.UserName} required Create on {org.Id}");
            }
        }

        public async Task<EntityOperationResult<OrganisationExtendedModel>> CreateOrganisationAsync(
            ClaimsPrincipal principal,
            OrganisationExtendedModel org)
        {
            // we do this when a new user signs up without an invite from an existing org 
            var user = await userService.UserManager.GetUserAsync(principal);
            if (user == null) return new EntityOperationResult<OrganisationExtendedModel>("Cannot find user. Please login");
            org.AddOrUpdateMembership(user, Roles.Administrator);
            // we good - create an org
            org = await Store.CreateAsync<OrganisationExtendedModel>(org);
            if (org != null)
            {
                // update user with org id
                try
                {
                    if (user.OrganisationId == null)
                    {
                        user.OrganisationId = org.OrganisationId; // set home org
                        var result = await userService.UserManager.UpdateAsync(user);
                        if (!result.Succeeded) throw new Exception("Failed to update user id");
                    }

                    return new EntityOperationResult<OrganisationExtendedModel>(org);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error creating org during onboarding. Will delete {org.Id}", ex);
                    // delete the org here incase something went wrong
                    await Store.DeleteAsync(org);
                    throw ex;
                }
            }
            else
            {
                return new EntityOperationResult<OrganisationExtendedModel>("Failed to create organisation");
            }

        }
    }
}