using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Models.Emails;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api.Services.Platform
{
    public class InvitationService : IInvitationService
    {
        private readonly IEntityStore<InvitationModel> invitationStore;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;
        private readonly bool isDevelopment;

        public InvitationService(IEntityStore<InvitationModel> invitationStore,
                                 IUserService userService,
                                 IEmailSender emailSender,
                                 IWebHostEnvironment env)
        {
            this.invitationStore = invitationStore;
            this.userService = userService;
            this.emailSender = emailSender;
            this.isDevelopment = env.IsDevelopment();
        }

        public IEntityStore<InvitationModel> Store => invitationStore;
        public async Task<EntityOperationResult<InvitationModel>> CreateInvitation(ClaimsPrincipal principal, InvitationModel invitation, bool inviteToOrg = false)
        {
            invitation.TargetEmail = invitation.TargetEmail.ToUpper(); // set uppercase
            var user = await userService.ReadUserModelAsync(principal);
            if (!user.EmailConfirmed && !isDevelopment)
            {
                return new EntityOperationResult<InvitationModel>($"{user.Email} must confirm email address to invite");
            }
            if (invitation.TargetOrganisationId != null && !user.IsAdmin())
            {
                return new EntityOperationResult<InvitationModel>($"{user.Email} must be an administrator to invite to an organisation");
            }
            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == user.NormalizedEmail);
            if (existing.Any())
            {
                var current = existing.FirstOrDefault();
                return new EntityOperationResult<InvitationModel>("User is already invited");
            }
            else
            {
                if (inviteToOrg) invitation.TargetOrganisationId = user.OrganisationId;
                invitation.CreatedDate = System.DateTime.UtcNow;
                invitation.LastModified = System.DateTime.UtcNow;
                var created = await invitationStore.CreateAsync(invitation);
                await emailSender.SendEmailAsync(new InvitationEmail(created));
                return new EntityOperationResult<InvitationModel>(created);
            }
        }

        public async Task<EntityOperationResult<IList<InvitationModel>>> GetMyInvitations(ClaimsPrincipal principal)
        {
            var user = await userService.ReadUserModelAsync(principal);
            if (user == null) return new EntityOperationResult<IList<InvitationModel>>("null user") { WasForbidden = true };
            var existing = await invitationStore.QueryAsync(i => i.TargetEmail == user.NormalizedEmail);
            if (existing.Count() > 0)
            {
                return new EntityOperationResult<IList<InvitationModel>>(new List<InvitationModel>(existing));
            }
            else
            {
                return new EntityOperationResult<IList<InvitationModel>>($"No Invitations found for {user.Email}");
            }
        }

        public async Task<EntityOperationResult<InvitationModel>> GetInvitation(ClaimsPrincipal principal, string orgId)
        {
            var user = await userService.ReadUserModelAsync(principal);
            var existing = await invitationStore.QueryAsync(i =>
                i.TargetEmail == user.NormalizedEmail
                && i.TargetOrganisationId == orgId);
            var invite = existing.FirstOrDefault();
            if (invite != null) return new EntityOperationResult<InvitationModel>(invite);
            else return new EntityOperationResult<InvitationModel>("Invitation does not exist");
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
            if (invitation.TargetOrganisationId == null) throw new System.ArgumentException("TargetOrganisationId cannot be null");

            var user = await userService.ReadUserModelAsync(principal);

            if (!string.Equals(user.NormalizedEmail, invitation.TargetEmail)) throw new System.ArgumentException("Emails do not match");

            user.OrganisationId = invitation.TargetOrganisationId;
            user.Organisation.AddOrUpdateMembership(user, Common.Models.Organisations.Roles.User);
            var res = await userService.UserManager.UpdateAsync(user);
            if (res.Succeeded)
            {
                invitation.IsClaimed = true;
                invitation = await invitationStore.UpdateAsync(invitation);
                return new EntityOperationResult<InvitationModel>(invitation);
            }
            else
            {
                return new EntityOperationResult<InvitationModel>(res.Errors?.Select(_ => _.Description));
            }

        }
    }
}