using Amphora.Common.Models.Platform;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Emails
{
    public class InvitationEmail : IEmail
    {
        public InvitationEmail(InvitationModel recipient)
        {
            this.Email = recipient.TargetEmail;
        }
        private const string templateId = "d-25458a4c163b4003aa5579bb328c281a";

        [JsonIgnore]
        public string SendGridTemplateId => templateId;
        [JsonIgnore]
        public string ToEmail => Email;
        [JsonIgnore]
        public string ToName { get; set; }
        
        // template data
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}