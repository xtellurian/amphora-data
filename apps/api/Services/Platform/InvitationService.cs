using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Emails;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Workflow.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api.Services.Platform
{
    public class InvitationService : IInvitationService
    {
        private readonly IEntityStore<InvitationModel> invitationStore;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPlanLimitService planLimitService;
        private readonly IUserDataService userDataService;
        private readonly IEmailSender emailSender;
        private readonly IWorkflows workflows;
        private readonly bool isDevelopment;

        public InvitationService(IEntityStore<InvitationModel> invitationStore,
                                 IEntityStore<OrganisationModel> orgStore,
                                 IPlanLimitService planLimitService,
                                 IUserDataService userDataService,
                                 IEmailSender emailSender,
                                 IWorkflows workflows,
                                 IWebHostEnvironment env)
        {
            this.invitationStore = invitationStore;
            this.orgStore = orgStore;
            this.planLimitService = planLimitService;
            this.userDataService = userDataService;
            this.emailSender = emailSender;
            this.workflows = workflows;
            this.isDevelopment = env.IsDevelopment();
        }

        public IEntityStore<InvitationModel> Store => invitationStore;
        public async Task<EntityOperationResult<InvitationModel>> CreateInvitation(ClaimsPrincipal principal, InvitationModel invitation)
        {
            invitation.TargetEmail = invitation.TargetEmail.ToLower(); // set uppercase
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<InvitationModel>(userReadRes.Message);
            }

            var userData = userReadRes.Entity;
            var org = await orgStore.ReadAsync(userData.OrganisationId);

            // check users within limits
            if (!await planLimitService.CanAddUser(org))
            {
                return new EntityOperationResult<InvitationModel>($"You have reached the user limit of your plan");
            }

            if (!principal.IsEmailConfirmed() && !isDevelopment)
            {
                return new EntityOperationResult<InvitationModel>(userData, $"{principal.GetUserName()} must confirm email address to invite");
            }

            if (invitation.TargetOrganisationId != null && !org.IsAdministrator(userData))
            {
                return new EntityOperationResult<InvitationModel>(userData, $"{principal.GetUserName()} must be an administrator to invite to an organisation");
            }

            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == invitation.TargetEmail &&
                i.TargetOrganisationId == invitation.TargetOrganisationId, 0, 3);

            if (existing.Any())
            {
                var current = existing.FirstOrDefault();
                return new EntityOperationResult<InvitationModel>(userData, "User is already invited");
            }
            else
            {
                invitation.TargetOrganisationId = userData.OrganisationId;
                invitation.CreatedDate = System.DateTime.UtcNow;
                invitation.LastModified = System.DateTime.UtcNow;
                var created = await invitationStore.CreateAsync(invitation);
                // send an invite email
                var templateData = InvitationEmail.GetTemplateData(created);
                var content = await emailSender.Generator.ContentFromMarkdownTemplateAsync(
                    InvitationEmail.TemplateName,
                    templateData);
                await emailSender.SendEmailAsync(new InvitationEmail(created, content));
                return new EntityOperationResult<InvitationModel>(userData, created);
            }
        }

        public async Task<EntityOperationResult<IList<InvitationModel>>> GetMyInvitations(ClaimsPrincipal principal)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<IList<InvitationModel>>(userReadRes.Message);
            }

            var userData = userReadRes.Entity;
            var email = principal.GetEmail();

            if (userData == null) { return new EntityOperationResult<IList<InvitationModel>>(userData, "null user") { WasForbidden = true }; }
            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == email, 0, 1);
            if (existing.Count() > 0)
            {
                return new EntityOperationResult<IList<InvitationModel>>(userData, new List<InvitationModel>(existing));
            }
            else
            {
                return new EntityOperationResult<IList<InvitationModel>>(userData, $"No Invitations found for {principal.GetUserName()}");
            }
        }

        public async Task<EntityOperationResult<InvitationModel>> GetInvitation(ClaimsPrincipal principal, string orgId)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<InvitationModel>(userReadRes.Message);
            }

            var userData = userReadRes.Entity;
            var email = principal.GetEmail();

            var existing = await invitationStore.QueryAsync(i =>
                i.TargetEmail == email
                && i.TargetOrganisationId == orgId, 0, 1);
            var invite = existing.FirstOrDefault();
            if (invite != null) { return new EntityOperationResult<InvitationModel>(userData, invite); }
            else { return new EntityOperationResult<InvitationModel>(userData, "Invitation does not exist"); }
        }

        public async Task<InvitationModel> GetInvitationByEmailAsync(string email)
        {
            email = email?.ToLower();
            var existing = await invitationStore.QueryAsync(i =>
                i.TargetEmail == email, 0, 5);
            var invite = existing.FirstOrDefault();
            if (invite == null)
            {
                var domain = email.Split('@')[1];
                existing = await invitationStore.QueryAsync(i =>
                    i.TargetDomain == domain, 0, 5);
                invite = existing.FirstOrDefault();
            }

            return invite;
        }

        public async Task<EntityOperationResult<InvitationModel>> HandleInvitationAsync(ClaimsPrincipal principal,
                                                                                        InvitationModel invitation,
                                                                                        InvitationTrigger trigger)
        {
            if (invitation.TargetOrganisationId == null) { throw new System.ArgumentException("TargetOrganisationId cannot be null"); }

            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<InvitationModel>(userReadRes.Message);
            }

            var userData = userReadRes.Entity;
            var email = principal.GetEmail();

            if (!string.Equals(email, invitation.TargetEmail)) { throw new System.ArgumentException("Emails do not match"); }

            if (trigger == InvitationTrigger.Accept)
            {
                // TODO: somehow move this into a workflow too
                userData.OrganisationId = invitation.TargetOrganisationId;
                invitation.TargetOrganisation.AddOrUpdateMembership(userData, Common.Models.Organisations.Roles.User);
                var res = await userDataService.UpdateAsync(principal, userData);
                if (res.Failed)
                {
                    return new EntityOperationResult<InvitationModel>(userData, res.Message);
                }
            }

            var wf = workflows.InvitationWorkflow(invitation);
            wf.Transition(trigger);
            invitation = await invitationStore.UpdateAsync(invitation);
            return new EntityOperationResult<InvitationModel>(userData, invitation);
        }
    }
}