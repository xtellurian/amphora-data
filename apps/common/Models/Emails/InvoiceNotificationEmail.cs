using System;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Emails
{
    public class InvoiceNotificationEmail : EmailBase, IEmail
    {
        public InvoiceNotificationEmail(ApplicationUserDataModel user, Invoice invoice)
        {
            if (user?.ContactInformation?.Email == null)
            {
                throw new ArgumentException($"{nameof(user)} email cannot be null");
            }

            Recipients.Add(new EmailRecipient(user?.ContactInformation?.Email!, user?.ContactInformation?.FullName!));
            // set user properties for email
            this.Name = user?.ContactInformation?.FullName ?? "User";
            this.Organisation = user?.Organisation?.Name ?? "Organisation";
            // set invoice properties for email
            this.CountCredits = invoice.CountCredits ?? 0;
            this.CountDebits = invoice.CountDebits ?? 0;

            this.TotalCredits = invoice.TotalCredits ?? 0;
            this.TotalDebits = invoice.TotalDebits ?? 0;
        }

        public override string SendGridTemplateId => "d-39a5a2ad988c4dc48371dd14c97dcc45";

        [JsonProperty("Name")]
        public string? Name { get; set; }
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
