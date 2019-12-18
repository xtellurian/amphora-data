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
        public string Id { get; set; }
        public virtual Account Account { get; set; } = null!;
        public string? Name { get; set; }
        public bool? IsPreview { get; set; } = true;
        public bool? IsPaid { get; set; } = false;
        public System.DateTimeOffset? DateCreated { get; set; }

        // Numbers
        public double? OpeningBalance { get; set; }
        public double InvoiceBalance => (TotalCredits ?? 0) - (TotalDebits ?? 0);
        public double? TotalCredits { get; set; }
        public double? TotalDebits { get; set; }

        // I know this seems weird, but when the list of credits or debits is empty, an exception is thrown
        // https://github.com/aspnet/EntityFrameworkCore/issues/19299
        public int? CountCredits { get; set; }
        public int? CountDebits { get; set; }

        public virtual ICollection<InvoiceCredit> Credits { get; set; } = new Collection<InvoiceCredit>();
        public virtual ICollection<InvoiceDebit> Debits { get; set; } = new Collection<InvoiceDebit>();
    }
}