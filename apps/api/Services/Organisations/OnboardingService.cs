using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Organisations
{
    // step 1: create user
    // step 2: create Org and asign user to that org
    public class OnboardingService : IOnboardingService
    {
        private readonly IEntityStore<OnboardingState> stateStore;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly ILogger<OnboardingService> logger;

        public OnboardingService(
            IEntityStore<OnboardingState> stateStore,
            IUserService userService,
            IPermissionService permissionService,
            IEntityStore<OrganisationModel> orgStore,
            ILogger<OnboardingService> logger)
        {
            this.stateStore = stateStore;
            this.userService = userService;
            this.permissionService = permissionService;
            this.orgStore = orgStore;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<IApplicationUser>> CreateUserAsync(IApplicationUser user, string password, string onboardingId = null)
        {
            OnboardingState state;
            user.IsOnboarding = true;
            if (string.IsNullOrEmpty(onboardingId))
            {
                state = new OnboardingState
                {
                    DateTimeCreated = DateTime.UtcNow,
                    IsNewOrganisation = true,
                    ttl = 60*60*24 // expire in 1 day
                };
                state = await stateStore.CreateAsync(state);
            }
            else
            {
                state = await stateStore.ReadAsync(onboardingId);
                if (state == null) return new EntityOperationResult<IApplicationUser>("Invalid Onboarding Id");
            }

            if(state.IsNewOrganisation == false)
            {
                // then we must attach to an existing org
                user.OrganisationId = state.OrganisationId;
            }
            if(user.OrganisationId == null && ! state.IsNewOrganisation)
            {
                return new EntityOperationResult<IApplicationUser>("OrganisationId cannot be null unless creating a new Organisation");
            }
            if(user.OrganisationId != null && state.IsNewOrganisation)
            {
                return new EntityOperationResult<IApplicationUser>("OrganisationId must not be null when creating a new Organisation");
            }
            user.OnboardingId = state.Id;
            var result = await userService.CreateAsync(user, password);
            if(result.Succeeded)
            {
                state.State = state.IsNewOrganisation ? OnboardingState.States.AwaitingOrganisation: OnboardingState.States.Completed;
                state.ConsumedByUserId = result.Entity.Id;
                await this.stateStore.UpdateAsync(state);
                if(state.State == OnboardingState.States.Completed)
                {
                    state.OrganisationId = user.OrganisationId;
                    user.IsOnboarding = false;
                    await userService.UserManager.UpdateAsync(user);
                }
                return new EntityOperationResult<IApplicationUser>(result.Entity);
            }
            else
            {
                logger.LogError("Failed to create user during onboarding");
                return new EntityOperationResult<IApplicationUser>(result.Errors);
            }
        }

        public async Task<EntityOperationResult<OrganisationModel>> CreateOrganisationAsync(ClaimsPrincipal principal, OrganisationModel org)
        {
            // we do this when a new user signs up without an invite from an existing org 
            var user = await userService.UserManager.GetUserAsync(principal);
            if(user == null) return new EntityOperationResult<OrganisationModel>("Cannot find user. Please login");
            var state = await stateStore.ReadAsync(user.OnboardingId);
            if(state == null)
            {
                return new EntityOperationResult<OrganisationModel>("Invalid onboarding Id");
            }

            if(string.Equals(state.State, OnboardingState.States.AwaitingOrganisation))
            {
                // we good - create an org
                org = await orgStore.CreateAsync(org);
                if(org != null)
                {
                    // update user with org id
                    try
                    {
                        user.OrganisationId = org.OrganisationId;
                        var result = await userService.UserManager.UpdateAsync(user);
                        if (!result.Succeeded) throw new Exception("Failed to update user id");
                        state.State = OnboardingState.States.Completed;
                        state.ConsumedByUserId = user.Id;
                        state.OrganisationId = user.OrganisationId;
                        state = await stateStore.UpdateAsync(state);

                        // give this user admin on the org
                        await permissionService.CreateOrganisationalRole(user, RoleAssignment.Roles.Administrator, org);

                        // close out onboarding
                        user.IsOnboarding = false;
                        await userService.UserManager.UpdateAsync(user);
                        return new EntityOperationResult<OrganisationModel>(org);
                    }
                    catch(Exception ex)
                    {
                        logger.LogError($"Error creating org during onboarding. Will delete {org.Id}", ex);
                        // delete the org here incase something went wrong
                        await orgStore.DeleteAsync(org);
                        throw ex;
                    }
                }
                else
                {
                    return new EntityOperationResult<OrganisationModel>("Failed to create organisation");
                }
            }
            else
            {
                return new EntityOperationResult<OrganisationModel>("Onboarding State did not expect Organisation Creation");
            }
        }

        public async Task<EntityOperationResult<OnboardingState>> InviteToOrganisation(ClaimsPrincipal principal, string email)
        {
            var invitingUser = await userService.UserManager.GetUserAsync(principal);
            if(invitingUser == null) return new EntityOperationResult<OnboardingState>("Inviting user doesn't exist.");
            var state = new OnboardingState()
            {
                State = OnboardingState.States.Invited,
                OrganisationId = invitingUser.OrganisationId,
                IsNewOrganisation = false,
                EmailToInvite = email
            };
            state = await stateStore.CreateAsync(state);
            if(state!=null)
            {
                var invitedUser = await userService.UserManager.FindByNameAsync(email);
                if(invitedUser != null && invitedUser.OrganisationId == null && invitedUser.IsOnboarding)
                {
                    invitedUser.OrganisationId = invitingUser.OrganisationId;
                    await userService.UserManager.UpdateAsync(invitedUser);

                    state.ConsumedByUserId = invitedUser.Id;
                    state.State = OnboardingState.States.Completed;
                    state = await stateStore.UpdateAsync(state);

                    var org = await orgStore.ReadAsync(invitedUser.OrganisationId);
                    await permissionService.CreateOrganisationalRole(invitedUser, RoleAssignment.Roles.User, org);
                }
                return new EntityOperationResult<OnboardingState>(state);
            }
            else
            {
                return new EntityOperationResult<OnboardingState>("Unknown Error");
            }
        }

        public async Task<OrganisationModel> GetOrganisationFromOnboardingId(string onboardingId)
        {
            var state = await stateStore.ReadAsync(onboardingId);
            if(state == null) return null;
            var org = await orgStore.ReadAsync(state.OrganisationId);
            return org;
        }
    }
}