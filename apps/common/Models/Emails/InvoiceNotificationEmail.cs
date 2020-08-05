using System;
using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class InvoiceNotificationEmail : EmailBase, IEmail
    {
        public static string TemplateName => "InvoiceNotification";
        public static Dictionary<string, string> GetTemplateData()
        {
            return new Dictionary<string, string>
            {
                { "{{Base_Url}}", BaseUrl }
            };
        }

        public InvoiceNotificationEmail(ApplicationUserDataModel user, string htmlContent)
        {
            if (user?.ContactInformation?.Email == null)
            {
                throw new ArgumentException($"{nameof(user)} email cannot be null");
            }

            Recipients.Add(new EmailRecipient(user?.ContactInformation?.Email!, user?.ContactInformation?.FullName!));
            HtmlContent = htmlContent;
        }

        public override string HtmlContent { get; set; }
    }
}
