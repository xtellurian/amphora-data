using System.Collections.Generic;
using System.Text.Encodings.Web;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Emails
{
    public class ConfirmEmailEmail : EmailBase, IEmail
    {
        public static string TemplateName => "ConfirmEmail";
        public static Dictionary<string, string> TemplateData(IUser user, string link)
        {
            return new Dictionary<string, string>
            {
                { "{{name}}", user.UserName ?? "Friend" },
                { "{{link}}", link }
            };
        }

        public ConfirmEmailEmail(string email, string userName, string htmlContent) : base("Please confirm your email address.")
        {
            Recipients.Add(new EmailRecipient(email, userName));
            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}