using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Organisations
{
    // step 1: create user
    // step 2: create Org and asign user to that org
    public class OrganisationService : IOrganisationService, IPermissionedEntityStore<OrganisationModel>
    {
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly IBlobStore<OrganisationModel> orgBlobStore;
        private readonly IEventPublisher eventPublisher;
        public IEntityStore<OrganisationModel> Store { get; }
        private readonly ILogger<OrganisationService> logger;

        public OrganisationService(
            IUserService userService,
            IPermissionService permissionService,
            IEntityStore<OrganisationModel> orgStore,
            IBlobStore<OrganisationModel> orgBlobStore,
            IEventPublisher eventPublisher,
            ILogger<OrganisationService> logger)
        {
            this.userService = userService;
            this.permissionService = permissionService;
            this.Store = orgStore;
            this.orgBlobStore = orgBlobStore;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<OrganisationModel>> CreateAsync(
            ClaimsPrincipal principal,
            OrganisationModel org)
        {
            org.WebsiteUrl = org.WebsiteUrl?.ToLower();
            // we do this when a new user signs up without an invite from an existing org
            var user = await userService.ReadUserModelAsync(principal);
            if (user == null) { return new EntityOperationResult<OrganisationModel>(user, "Cannot find user. Please login"); }
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                if (await Store.CountAsync(_ => _.WebsiteUrl == org.WebsiteUrl) > 0)
                {
                    var existing = (await Store.QueryAsync(_ => _.WebsiteUrl == org.WebsiteUrl)).FirstOrDefault();
                    // an organisation with that website already exists.
                    return new EntityOperationResult<OrganisationModel>(user, 409,
                        $"An organisation called {existing?.Name} with website {existing?.WebsiteUrl} already exists. You may wish to join that org.");
                }

                org.CreatedById = user.Id;
                org.CreatedBy = user;
                org.CreatedDate = DateTime.UtcNow;
                org.LastModified = DateTime.UtcNow;

                if (!org.IsValid()) { throw new ArgumentException("Organisation is Invalid"); }
                org.AddOrUpdateMembership(user, Roles.Administrator);
                // we good - create an org
                org = await Store.CreateAsync(org);
                await eventPublisher.PublishEventAsync(new OrganisationCreatedEvent(org));
                logger.LogTrace($"Organisation Created. Name: {org.Name}, Id: {org.Id}");
                if (org != null)
                {
                    // update user with org id
                    try
                    {
                        if (string.IsNullOrEmpty(user.OrganisationId))
                        {
                            user.OrganisationId = org.Id; // set home org
                            var result = await userService.UpdateAsync(principal, user);
                        }

                        logger.LogInformation($"Assigned user to organisation, Name: {org.Name}, Id: {org.Id}");
                        return new EntityOperationResult<OrganisationModel>(user, org);
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
                    return new EntityOperationResult<OrganisationModel>(user, "Failed to create organisation");
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> ReadAsync(ClaimsPrincipal principal, string id)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<OrganisationService>(user)))
            {
                var org = await Store.ReadAsync(id);
                if (org == null) { return new EntityOperationResult<OrganisationModel>(user, $"{id} not found"); }
                var authorized = await permissionService.IsAuthorizedAsync(user, org, AccessLevels.Read);
                if (authorized)
                {
                    logger.LogInformation($"User {user.UserName} reads Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>(user, org);
                }
                else
                {
                    logger.LogInformation($"User {user.UserName} denied to read Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>(user, $"User {user.UserName} needs read access to Org {org.Id}");
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
                    return new EntityOperationResult<OrganisationModel>(user, org);
                }
                else
                {
                    logger.LogWarning($"Permission denied to update organisation, Name: {org.Name}, Id: {org.Id}");
                    return new EntityOperationResult<OrganisationModel>() { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> DeleteAsync(ClaimsPrincipal principal, OrganisationModel model)
        {
            var userId = principal.GetUserId();

            if (model.IsAdministrator(userId))
            {
                await this.DeleteRelated(model);
                await Store.DeleteAsync(model);
                return new EntityOperationResult<OrganisationModel>(true);
            }
            else
            {
                return new EntityOperationResult<OrganisationModel>("Only Administrators can delete.");
            }
        }

        private async Task DeleteRelated(OrganisationModel model)
        {
            // need to delete all related restrictions
            model.Restrictions.Clear();
            model.TargetedByRestrictions.Clear();
            await Store.UpdateAsync(model);
        }

        public async Task<EntityOperationResult<TermsAndConditionsAcceptanceModel>> AgreeToTermsAndConditions(ClaimsPrincipal principal, TermsAndConditionsModel termsAndConditions)
        {
            var user = await userService.ReadUserModelAsync(principal);
            var org = await this.Store.ReadAsync(user.OrganisationId);

            if (org != null && org.IsAdministrator(user))
            {
                org.TermsAndConditionsAccepted ??= new List<TermsAndConditionsAcceptanceModel>();

                var model = new TermsAndConditionsAcceptanceModel(org, termsAndConditions);
                org.TermsAndConditionsAccepted.Add(model);
                var o = await Store.UpdateAsync(org);
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(user, model);
            }
            else
            {
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(user, $"User {user.UserName} must be an administrator");
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
