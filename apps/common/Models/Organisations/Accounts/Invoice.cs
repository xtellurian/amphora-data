using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public System.DateTimeOffset? Timestamp { get; set; }

        // Numbers
        public double? OpeningBalance { get; set; }
        public double InvoiceBalance => (TotalCredits ?? 0) - (TotalDebits ?? 0);
        public double? TotalCredits { get; set; }
        public double? TotalDebits { get; set; }

        // I know this seems weird, but when the list of credits or debits is empty, an exception is thrown
        // https://github.com/aspnet/EntityFrameworkCore/issues/19299
        public int? CountCredits => Credits.Count();
        public int? CountDebits => Debits.Count();

        public virtual IEnumerable<InvoiceTransaction> Credits => Transactions.Where(c => c.IsCredit == true).ToList();
        public virtual IEnumerable<InvoiceTransaction> Debits => Transactions.Where(c => c.IsDebit == true).ToList();
        public virtual ICollection<InvoiceTransaction> Transactions { get; set; } = new Collection<InvoiceTransaction>();
    }
}