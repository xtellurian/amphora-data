using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Account
    {
        public virtual ICollection<AccountCredit> Credits { get; set; } = new Collection<AccountCredit>();
        public virtual ICollection<AccountDebit> Debits { get; set; } = new Collection<AccountDebit>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new Collection<Invoice>();
        public virtual OrganisationModel Organisation { get; set; } = null!;
        public string OrganisationId { get; set; } = null!;
        public double? Balance => GetBalance();

        public IList<Invoice> GetUnpaidInvoices(bool includePreview = false)
        {
            IList<Invoice> unpaidInvoices;
            if (includePreview)
            {
                unpaidInvoices = this.Invoices.Where(_ => _.IsPaid.HasValue
                    && !_.IsPaid.Value).ToList();
            }
            else
            {
                unpaidInvoices = this.Invoices.Where(_ => _.IsPaid.HasValue
                    && !_.IsPaid.Value
                    && _.IsPreview.HasValue
                    && !_.IsPreview.Value).ToList();
            }

            return unpaidInvoices;
        }

        public IList<Invoice> GetPaidInvoices(bool includePreview = false)
        {
            IList<Invoice> paidInvoices;
            if (includePreview)
            {
                paidInvoices = this.Invoices.Where(_ => _.IsPaid.HasValue
                && _.IsPaid.Value).ToList();
            }
            else
            {
                paidInvoices = this.Invoices.Where(_ => _.IsPaid.HasValue
                    && _.IsPaid.Value
                    && _.IsPreview.HasValue
                    && !_.IsPreview.Value).ToList();
            }

            return paidInvoices;
        }

        public double? GetBalance()
        {
            var credit = this.Credits?.Sum(c => c.Amount) ?? 0;
            var debit = this.Debits?.Sum(d => d.Amount) ?? 0;
            return credit - debit;
        }

        public void DebitAccount(string label, double amount, string? amphoraId = null)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }

            if (Debits == null) { Debits = new List<AccountDebit>(); }
            if (amphoraId == null) { Debits.Add(new AccountDebit(label, amount)); }
            else { Debits.Add(new AccountDebit(label, amount, amphoraId)); }
        }

        public void CreditAccount(string label, double amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }

            if (Credits == null) { Credits = new List<AccountCredit>(); }
            Credits.Add(new AccountCredit(label, amount));
        }
    }
}