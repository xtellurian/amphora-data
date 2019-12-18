using System;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Api.Models.Emails
{
    public class InvoiceNotificationEmail : IEmail
    {
        public InvoiceNotificationEmail(ApplicationUser user, Invoice invoice)
        {
            this.ToEmail = user.Email;
            this.ToName = user.FullName;
            if (!user.IsAdmin()) throw new ArgumentException("User is not an administrator");
            this.ToName = user.FullName;
            // set user properties for email
            this.Name = user.FullName;
            this.Organisation = user.Organisation.Name;
            // set invoice properties for email
            this.CountCredits = invoice.CountCredits ?? 0;
            this.CountDebits = invoice.CountDebits ?? 0;

            this.TotalCredits = invoice.TotalCredits ?? 0;
            this.TotalDebits = invoice.TotalDebits ?? 0;

        }

        public string SendGridTemplateId => "d-39a5a2ad988c4dc48371dd14c97dcc45";

        public string ToEmail { get; private set; }

        public string ToName { get; private set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Organisation")]
        public string Organisation { get; set; }
        [JsonProperty("CountDebits")]
        public int CountDebits { get; set; }
        [JsonProperty("CountCredits")]
        public int CountCredits { get; set; }
        [JsonProperty("TotalDebits")]
        public double TotalDebits { get; set; }
        [JsonProperty("TotalCredits")]
        public double TotalCredits { get; set; }
        [JsonProperty("OpeningBalance")]
        public double OpeningBalance { get; set; }
        [JsonProperty("InvoiceBalance")]
        public double InvoiceBalance { get; set; }

    }
}
