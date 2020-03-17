using System.Text.Encodings.Web;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Host;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Emails
{
    public class ConfirmEmailEmail : EmailBase, IEmail
    {
        private string page = "Profiles/Account/ConfirmEmail";
        public ConfirmEmailEmail(ApplicationUserDataModel user, HostOptions options, string code)
        {
            Recipients.Add(new EmailRecipient(user.ContactInformation.Email, user.ContactInformation.FullName));
            this.Name = user.ContactInformation.FullName;
            Link = HtmlEncoder.Default.Encode($"{options.GetBaseUrl()}{page}?userId={user.Id}&code={code}");
        }

        public override string SendGridTemplateId => "d-b6be9fdd4d49426ca958b83c166f3d1f";

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}