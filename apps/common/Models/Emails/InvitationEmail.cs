using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class InvitationEmail : EmailBase, IEmail
    {
        public InvitationEmail(InvitationModel recipient, string? baseUrl = null)
        {
            if (recipient is null || recipient.TargetEmail is null)
            {
                throw new System.ArgumentNullException(nameof(recipient) + " or recipient.TargetEmail");
            }

            this.Recipients.Add(new EmailRecipient(recipient.TargetEmail, ""));
            this.Email = recipient.TargetEmail;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                BaseUrl = baseUrl;
            }
            else
            {
                BaseUrl = "https://app.amphoradata.com";
            }
        }

        [JsonIgnore]
        public override string SendGridTemplateId => "d-25458a4c163b4003aa5579bb328c281a";

        // template data
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
