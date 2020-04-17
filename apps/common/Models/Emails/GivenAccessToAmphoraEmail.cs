using System.Text.Encodings.Web;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Host;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class GivenAccessToAmphoraEmail : EmailBase, IEmail
    {
        public GivenAccessToAmphoraEmail(AmphoraModel amphora, string targetEmail, string? fullName = null)
        {
            AmphoraUrl = HtmlEncoder.Default.Encode($"{BaseUrl}/Amphorae/Detail?id={amphora.Id}");
            this.Recipients.Add(new EmailRecipient(targetEmail, fullName ?? ""));
        }

        public override string SendGridTemplateId => " d-07e70362967f4436958b24e876f3bb87";

        [JsonProperty("amphoraUrl")]
        public string AmphoraUrl { get; set; }
    }
}