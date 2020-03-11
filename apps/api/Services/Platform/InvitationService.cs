using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Models.Emails;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api.Services.Platform
{
    public class InvitationService : IInvitationService
    {
        private readonly IEntityStore<InvitationModel> invitationStore;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;
        private readonly bool isDevelopment;

        public InvitationService(IEntityStore<InvitationModel> invitationStore,
                                 IEntityStore<OrganisationModel> orgStore,
                                 IUserService userService,
                                 IEmailSender emailSender,
                                 IWebHostEnvironment env)
        {
            this.invitationStore = invitationStore;
            this.orgStore = orgStore;
            this.userService = userService;
            this.emailSender = emailSender;
            this.isDevelopment = env.IsDevelopment();
        }

        public IEntityStore<InvitationModel> Store => invitationStore;
        public async Task<EntityOperationResult<InvitationModel>> CreateInvitation(ClaimsPrincipal principal, InvitationModel invitation, bool inviteToOrg = false)
        {
            invitation.TargetEmail = invitation.TargetEmail.ToUpper(); // set uppercase
            var user = await userService.ReadUserModelAsync(principal);
            var org = await orgStore.ReadAsync(user.OrganisationId);

            if (!user.EmailConfirmed && !isDevelopment)
            {
                return new EntityOperationResult<InvitationModel>(user, $"{user.Email} must confirm email address to invite");
            }

            if (invitation.TargetOrganisationId != null && !org.IsAdministrator(user))
            {
                return new EntityOperationResult<InvitationModel>(user, $"{user.Email} must be an administrator to invite to an organisation");
            }

            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == invitation.TargetEmail);
            if (existing.Any())
            {
                var current = existing.FirstOrDefault();
                return new EntityOperationResult<InvitationModel>(user, "User is already invited");
            }
            else
            {
                if (inviteToOrg) { invitation.TargetOrganisationId = user.OrganisationId; }
                invitation.CreatedDate = System.DateTime.UtcNow;
                invitation.LastModified = System.DateTime.UtcNow;
                var created = await invitationStore.CreateAsync(invitation);
                // send an invite email
                await emailSender.SendEmailAsync(new InvitationEmail(created));
                return new EntityOperationResult<InvitationModel>(user, created);
            }
        }

        public async Task<EntityOperationResult<IList<InvitationModel>>> GetMyInvitations(ClaimsPrincipal principal)
        {
            var user = await userService.ReadUserModelAsync(principal);
            if (user == null) { return new EntityOperationResult<IList<InvitationModel>>(user, "null user") { WasForbidden = true }; }
            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == user.NormalizedEmail);
            if (existing.Count() > 0)
            {
                return new EntityOperationResult<IList<InvitationModel>>(user, new List<InvitationModel>(existing));
            }
            else
            {
                return new EntityOperationResult<IList<InvitationModel>>(user, $"No Invitations found for {user.Email}");
            }
        }

        public async Task<EntityOperationResult<InvitationModel>> GetInvitation(ClaimsPrincipal principal, string orgId)
        {
            var user = await userService.ReadUserModelAsync(principal);
            var existing = await invitationStore.QueryAsync(i =>
                i.TargetEmail == user.NormalizedEmail
                && i.TargetOrganisationId == orgId);
            var invite = existing.FirstOrDefault();
            if (invite != null) { return new EntityOperationResult<InvitationModel>(user, invite); }
            else { return new EntityOperationResult<InvitationModel>(user, "Invitation does not exist"); }
        }

        public async Task<InvitationModel> GetInvitationByEmailAsync(string email)
        {
            email = email?.ToUpper();
            var existing = await invitationStore.QueryAsync(i =>
                i.TargetEmail == email);
            var invite = existing.FirstOrDefault();
            if (invite == null)
            {
                var domain = email.Split('@')[1];
                existing = await invitationStore.QueryAsync(i =>
                    i.TargetDomain == domain);
                invite = existing.FirstOrDefault();
            }

            return invite;
        }

        public async Task<EntityOperationResult<InvitationModel>> AcceptInvitationAsync(ClaimsPrincipal principal, InvitationModel invitation)
        {
            if (invitation.TargetOrganisationId == null) { throw new System.ArgumentException("TargetOrganisationId cannot be null"); }

            var user = await userService.ReadUserModelAsync(principal);

            if (!string.Equals(user.NormalizedEmail, invitation.TargetEmail)) { throw new System.ArgumentException("Emails do not match"); }

            user.OrganisationId = invitation.TargetOrganisationId;
            user.Organisation.AddOrUpdateMembership(user, Common.Models.Organisations.Roles.User);
            var res = await userService.UpdateAsync(principal, user);
            if (res.Succeeded)
            {
                invitation.IsClaimed = true;
                invitation = await invitationStore.UpdateAsync(invitation);
                return new EntityOperationResult<InvitationModel>(user, invitation);
            }
            else
            {
                return new EntityOperationResult<InvitationModel>(user, res.Errors);
            }
        }
    }
}