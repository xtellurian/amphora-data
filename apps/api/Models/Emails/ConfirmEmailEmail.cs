
using Amphora.Api.Models.Host;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Emails
{
    public class ConfirmEmailEmail : IEmail
    {
        private string page = "Profiles/Account/ConfirmEmail";
        public ConfirmEmailEmail(ApplicationUser user, HostOptions options, string code)
        {
            this.Name = user.FullName;
            this.ToEmail = user.Email;
            this.Link = $"{options.GetBaseUrl()}{page}?userId={user.Id}&code={code}";
        }
        public string SendGridTemplateId => "d-b6be9fdd4d49426ca958b83c166f3d1f";

        public string ToEmail { get; private set; }

        public string ToName => Name;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}