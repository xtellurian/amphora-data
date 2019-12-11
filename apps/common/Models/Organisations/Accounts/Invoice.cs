using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Invoice
    {
        public Invoice(string id)
        {
            Id = id;
        }
        public Invoice()
        {
            Id = null!;
        }

        public double Balance { get; set; }
        public string Id { get; set; }
        public virtual Account Account { get; set; } = null!;
        public string? Name { get; set; }
        public bool? IsPreview { get; set; } = true;
        public System.DateTimeOffset? DateCreated { get; set; }
        // I know this seems weird, but when the list of credits or debits is empty, an exception is thrown
        public int? NumberOfCredits { get; set; }
        public int? NumberOfDebits { get; set; }
        public virtual ICollection<InvoiceCredit> Credits { get; set; } = new Collection<InvoiceCredit>();
        public virtual ICollection<InvoiceDebit> Debits { get; set; } = new Collection<InvoiceDebit>();
    }
}