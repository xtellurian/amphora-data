using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Emails
{
    public class InvitationEmail : EmailBase, IEmail
    {
        public InvitationEmail(InvitationModel recipient, string baseUrl = null)
        {
            this.Recipients.Add(new EmailRecipient(recipient.TargetEmail, ""));
            this.Email = recipient.TargetEmail;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                BaseUrl = baseUrl;
            }
            else
            {
                BaseUrl = "https://identity.amphoradata.com"; // invitations are sent by the identity
            }
        }

        [JsonIgnore]
        public override string SendGridTemplateId => "d-25458a4c163b4003aa5579bb328c281a";

        // template data
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
