using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Exceptions;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;
using System.Linq;
using Amphora.Common.Models.Permissions;
using System.Collections.Generic;

namespace Amphora.Api.Services.Organisations
{
    // step 1: create user
    // step 2: create Org and asign user to that org
    public class OrganisationService : IOrganisationService
    {
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly IBlobStore<OrganisationModel> orgBlobStore;

        public IEntityStore<OrganisationModel> Store { get; }
        private readonly ILogger<OrganisationService> logger;

        public OrganisationService(
            IUserService userService,
            IPermissionService permissionService,
            IEntityStore<OrganisationModel> orgStore,
            IBlobStore<OrganisationModel> orgBlobStore,
            ILogger<OrganisationService> logger)
        {
            this.userService = userService;
            this.permissionService = permissionService;
            this.Store = orgStore;
            this.orgBlobStore = orgBlobStore;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<OrganisationModel>> CreateOrganisationAsync(
            ClaimsPrincipal principal,
            OrganisationModel org)
        {
            // we do this when a new user signs up without an invite from an existing org 
            var user = await userService.UserManager.GetUserAsync(principal);
            if (user == null) return new EntityOperationResult<OrganisationModel>("Cannot find user. Please login");
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                org.CreatedById = user.Id;
                org.CreatedBy = user;
                org.CreatedDate = DateTime.UtcNow;
                if (!org.IsValid()) throw new ArgumentException("Organisation is Invalid");
                org.AddOrUpdateMembership(user, Roles.Administrator);
                // we good - create an org
                org = await Store.CreateAsync(org);
                logger.LogTrace($"Organisation Created. Name: {org.Name}, Id: {org.Id}");
                if (org != null)
                {
                    // update user with org id
                    try
                    {
                        if (string.IsNullOrEmpty(user.OrganisationId))
                        {
                            user.OrganisationId = org.Id; // set home org
                            var result = await userService.UserManager.UpdateAsync(user);
                        }
                        logger.LogInformation($"Assigned user to organisation, Name: {org.Name}, Id: {org.Id}");
                        return new EntityOperationResult<OrganisationModel>(org);
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
                    return new EntityOperationResult<OrganisationModel>("Failed to create organisation");
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> ReadAsync(ClaimsPrincipal principal, string id)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                var org = await Store.ReadAsync(id);
                if (org == null) return new EntityOperationResult<OrganisationModel>($"{id} not found");
                var authorized = await permissionService.IsAuthorizedAsync(user, org, AccessLevels.Read);
                if (authorized)
                {
                    logger.LogInformation($"User {user.UserName} reads Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>(org);
                }
                else
                {
                    logger.LogInformation($"User {user.UserName} denied to read Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>($"User {user.UserName} needs read access to Org {org.Id}");
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> UpdateAsync(ClaimsPrincipal principal, OrganisationModel org)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                var authorized = await permissionService.IsAuthorizedAsync(user, org, AccessLevels.Update);
                if (authorized)
                {
                    logger.LogTrace("Updating organisation, Name: {org.Name}, Id: {org.Id}");
                    org = await this.Store.UpdateAsync(org);
                    return new EntityOperationResult<OrganisationModel>(org);
                }
                else
                {
                    logger.LogWarning($"Permission denied to update organisation, Name: {org.Name}, Id: {org.Id}");
                    return new EntityOperationResult<OrganisationModel>() { WasForbidden = true };
                }
            }
        }
        public async Task<bool> AcceptInvitation(ClaimsPrincipal principal, string orgId)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                var org = await Store.ReadAsync(orgId);
                var invitation = org.Invitations?.FirstOrDefault(i => string.Equals(i.TargetEmail.ToUpper(), user.Email.ToUpper()));

                if (invitation != null)
                {
                    user.OrganisationId = org.Id;
                    var result = await userService.UserManager.UpdateAsync(user);
                    if (result != null)
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
        }

        public async Task<EntityOperationResult<TermsAndConditionsAcceptanceModel>> AgreeToTermsAndConditions(ClaimsPrincipal principal, TermsAndConditionsModel termsAndConditions)
        {
            var user = await userService.ReadUserModelAsync(principal);
            if (user.IsAdmin())
            {
                if (user.Organisation.TermsAndConditionsAccepted == null)
                {
                    user.Organisation.TermsAndConditionsAccepted = new List<TermsAndConditionsAcceptanceModel>();
                }
                var model = new TermsAndConditionsAcceptanceModel(user.Organisation, termsAndConditions);
                user.Organisation.TermsAndConditionsAccepted.Add(model);
                var o = await Store.UpdateAsync(user.Organisation);
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(model);
            }
            else
            {
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>($"User {user.UserName} must be an administrator");
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> InviteToOrganisationAsync(ClaimsPrincipal principal, string orgId, string email)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                var org = await Store.ReadAsync(orgId);
                var authorized = await permissionService.IsAuthorizedAsync(user, org, ResourcePermissions.Create);
                if (authorized)
                {
                    var invitation = new Invitation(email);
                    org.AddInvitation(email);
                    var result = await Store.UpdateAsync(org);
                    return new EntityOperationResult<OrganisationModel>(result);
                }
                else
                {
                    logger.LogError($"{user.UserName} required Create on {org.Id}");
                    return new EntityOperationResult<OrganisationModel>($"{user.UserName} required Create on {org.Id}") { WasForbidden = true };
                }
            }
        }

        public async Task WriteProfilePictureJpg(OrganisationModel organisation, byte[] bytes)
        {
            await orgBlobStore.WriteBytesAsync(organisation, "profile.jpg", bytes);
        }

        public async Task<byte[]> ReadrofilePictureJpg(OrganisationModel organisation)
        {
            return await orgBlobStore.ReadBytesAsync(organisation, "profile.jpg") ?? new byte[0];
        }
    }
}
