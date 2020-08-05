using System.Collections.Generic;
using System.Text.Encodings.Web;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Emails
{
    public class ConfirmEmailEmail : EmailBase, IEmail
    {
        public static string TemplateName => "ConfirmEmail";
        private static string page = "Profiles/Account/ConfirmEmail";
        public static Dictionary<string, string> TemplateData(ApplicationUserDataModel user, string baseUrl, string code)
        {
            return new Dictionary<string, string>
            {
                { "{{name}}", user.UserName ?? "Friend" },
                { "{{link}}", HtmlEncoder.Default.Encode($"{baseUrl}/{page}?userId={user?.Id}&code={code}") }
            };
        }

        public ConfirmEmailEmail(ApplicationUserDataModel user, string htmlContent) : base("Please confirm your email address.")
        {
            Recipients.Add(new EmailRecipient(user?.ContactInformation?.Email!, user?.ContactInformation?.FullName!));
            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}