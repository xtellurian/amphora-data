using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Emails
{
    public class WelcomeEmail : EmailBase, IEmail
    {
        public static string TemplateName => "Welcome";
        public static Dictionary<string, string> TemplateData(IUser user)
        {
            return new Dictionary<string, string>
            {
                { "{{name}}", user.UserName ?? "Friend" },
                { "{{base_url}}", BaseUrl }
            };
        }

        public WelcomeEmail(string email, string userName, string htmlContent) : base("Welcome to Amphora")
        {
            Recipients.Add(new EmailRecipient(email, userName));
            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}