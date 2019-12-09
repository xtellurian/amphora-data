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

        public double Balance => GetBalance();
        public double GetBalance()
        {
            var credit = this.Credits?.Sum(c => c.Amount) ?? 0;
            var debit = this.Debits?.Sum(d => d.Amount) ?? 0;
            return credit - debit;
        }

        public string Id { get; set; }
        public virtual Account Account { get; set; } = null!;
        public string? Name { get; set; }
        public System.DateTimeOffset? DateCreated { get; set; }
        public virtual ICollection<InvoiceCredit> Credits { get; set; } = new Collection<InvoiceCredit>();
        public virtual ICollection<InvoiceDebit> Debits { get; set; } = new Collection<InvoiceDebit>();
    }
}