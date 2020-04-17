using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class RequestToJoinEmail : EmailBase, IEmail
    {
        public RequestToJoinEmail(ApplicationUserDataModel requester, OrganisationModel org)
        {
            this.OrganisationName = org.Name;
            this.InviteeEmail = requester.ContactInformation.Email;
            this.InviteeName = requester.ContactInformation.FullName;

            // send this to the org owner and/or admins
            foreach (var member in org.Memberships)
            {
                if (member.User?.ContactInformation?.Email != null && member.Role == Roles.Administrator)
                {
                    Recipients.Add(new EmailRecipient(member.User?.ContactInformation?.Email, member.User?.ContactInformation?.FullName ?? "Amphora User"));
                }
            }
        }

        public override string SendGridTemplateId => "d-f9c2d344c20046ada377a2032f339e77";

        [JsonProperty("inviteeName")]
        public string InviteeName { get; set; }
        [JsonProperty("inviteeEmail")]
        public string InviteeEmail { get; set; }
        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; }
    }
}