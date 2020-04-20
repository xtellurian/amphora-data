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
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly IBlobStore<OrganisationModel> orgBlobStore;
        private readonly IEventRoot eventPublisher;
        public IEntityStore<OrganisationModel> Store { get; }
        private readonly ILogger<OrganisationService> logger;

        public OrganisationService(
            IUserDataService userDataService,
            IPermissionService permissionService,
            IEntityStore<OrganisationModel> orgStore,
            IBlobStore<OrganisationModel> orgBlobStore,
            IEventRoot eventPublisher,
            ILogger<OrganisationService> logger)
        {
            this.userDataService = userDataService;
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
            var userId = principal.GetUserId();
            // we do this when a new user signs up without an invite from an existing org
            var userReadRes = await userDataService.ReadAsync(principal, userId);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<OrganisationModel>("Unknown User Id");
            }

            var userData = userReadRes.Entity;

            if (userData == null) { return new EntityOperationResult<OrganisationModel>("Cannot find user. Please login"); }
            using (logger.BeginScope(new LoggerScope<OrganisationService>(principal)))
            {
                if (await Store.CountAsync(_ => _.WebsiteUrl != null && _.WebsiteUrl == org.WebsiteUrl) > 0)
                {
                    var existing = (await Store.QueryAsync(_ => _.WebsiteUrl == org.WebsiteUrl)).FirstOrDefault();
                    // an organisation with that website already exists.
                    return new EntityOperationResult<OrganisationModel>(userData,
                        $"An organisation called {existing?.Name} with website {existing?.WebsiteUrl} already exists. You may wish to join that org.");
                }

                org.CreatedById = userData.Id;
                org.CreatedBy = userData;
                org.CreatedDate = DateTime.UtcNow;
                org.LastModified = DateTime.UtcNow;

                if (!org.IsValid()) { throw new ArgumentException("Organisation is Invalid"); }
                org.AddOrUpdateMembership(userData, Roles.Administrator);
                // we good - create an org
                org = await Store.CreateAsync(org);
                await eventPublisher.PublishEventAsync(new OrganisationCreatedEvent(org));
                logger.LogTrace($"Organisation Created. Name: {org.Name}, Id: {org.Id}");
                if (org != null)
                {
                    // update user with org id
                    try
                    {
                        if (string.IsNullOrEmpty(userData.OrganisationId))
                        {
                            userData.OrganisationId = org.Id; // set home org
                            var result = await userDataService.UpdateAsync(principal, userData);
                        }

                        logger.LogInformation($"Assigned user to organisation, Name: {org.Name}, Id: {org.Id}");
                        return new EntityOperationResult<OrganisationModel>(userData, org);
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
                    return new EntityOperationResult<OrganisationModel>(userData, "Failed to create organisation");
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> ReadAsync(ClaimsPrincipal principal, string id)
        {
            var userId = principal.GetUserId();
            var userReadRes = await userDataService.ReadAsync(principal, userId);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<OrganisationModel>(false);
            }

            var userData = userReadRes.Entity;

            using (logger.BeginScope(new LoggerScope<OrganisationService>(principal)))
            {
                var org = await Store.ReadAsync(id);
                if (org == null) { return new EntityOperationResult<OrganisationModel>(userData, $"{id} not found"); }
                var authorized = await permissionService.IsAuthorizedAsync(userData, org, AccessLevels.Read);
                if (authorized)
                {
                    logger.LogInformation($"User {principal.GetUserName()} reads Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>(userData, org);
                }
                else
                {
                    logger.LogInformation($"User {principal.GetUserName()} denied to read Organisation {org.Name}");
                    return new EntityOperationResult<OrganisationModel>(userData, $"User {principal.GetUserName()} needs read access to Org {org.Id}");
                }
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> UpdateAsync(ClaimsPrincipal principal, OrganisationModel org)
        {
            var userId = principal.GetUserId();
            var readUserRes = await userDataService.ReadAsync(principal, userId);
            if (!readUserRes.Succeeded)
            {
                return new EntityOperationResult<OrganisationModel>(false);
            }

            var userData = readUserRes.Entity;

            using (logger.BeginScope(new LoggerScope<OrganisationService>(principal)))
            {
                var authorized = await permissionService.IsAuthorizedAsync(userData, org, AccessLevels.Update);
                if (authorized)
                {
                    logger.LogTrace("Updating organisation, Name: {org.Name}, Id: {org.Id}");
                    org = await this.Store.UpdateAsync(org);
                    return new EntityOperationResult<OrganisationModel>(userData, org);
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
                await Store.DeleteAsync(model);
                return new EntityOperationResult<OrganisationModel>(true);
            }
            else
            {
                return new EntityOperationResult<OrganisationModel>("Only Administrators can delete.");
            }
        }

        public async Task<EntityOperationResult<TermsAndConditionsAcceptanceModel>> AgreeToTermsAndConditions(ClaimsPrincipal principal, TermsAndConditionsModel termsAndConditions)
        {
            var userId = principal.GetUserId();
            var userReadRes = await userDataService.ReadAsync(principal, userId);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(false);
            }

            var userData = userReadRes.Entity;
            var org = await this.Store.ReadAsync(userData.OrganisationId);

            if (org != null && org.IsAdministrator(userData))
            {
                org.TermsAndConditionsAccepted ??= new List<TermsAndConditionsAcceptanceModel>();

                var model = new TermsAndConditionsAcceptanceModel(org, termsAndConditions);
                org.TermsAndConditionsAccepted.Add(model);
                var o = await Store.UpdateAsync(org);
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(userData, model);
            }
            else
            {
                return new EntityOperationResult<TermsAndConditionsAcceptanceModel>(userData, $"User {principal.GetUserName()} must be an administrator");
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
