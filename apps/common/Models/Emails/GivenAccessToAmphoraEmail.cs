using System.Collections.Generic;
using System.Text.Encodings.Web;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Models.Emails
{
    public class GivenAccessToAmphoraEmail : EmailBase, IEmail
    {
        public static string TemplateName => "GivenAccessToAmphora";
        public static Dictionary<string, string> GetTemplateData(AmphoraModel amphora) => new Dictionary<string, string>
        {
            { "{{name}}", amphora.Name },
            { "{{amphora_url}}", HtmlEncoder.Default.Encode($"{BaseUrl}/Amphorae/Detail?id={amphora.Id}") }
        };

        public GivenAccessToAmphoraEmail(string htmlContent, string? targetEmail, string? name) : base("New data!")
        {
            if (!string.IsNullOrEmpty(targetEmail))
            {
                this.Recipients.Add(new EmailRecipient(targetEmail, name ?? "Friend"));
            }

            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}