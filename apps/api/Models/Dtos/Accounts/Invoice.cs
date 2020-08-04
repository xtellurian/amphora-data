using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Accounts
{
    public class Invoice : IDto
    {
        public string Id { get; set; }
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public bool? IsPaid { get; set; }
        public System.DateTimeOffset? DateCreated { get; set; }
        public System.DateTimeOffset? Timestamp { get; set; }

        // Numbers
        public double OpeningBalance { get; set; }
        public double InvoiceBalance { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}