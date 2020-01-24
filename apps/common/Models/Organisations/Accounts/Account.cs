using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Common.Models.Purchases;

namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Account
    {
        private const double DefaultCommissionRate = 0.8; // i.e. the fraction kept by the provider
        public virtual ICollection<AccountCredit> Credits { get; set; } = new Collection<AccountCredit>();
        public virtual ICollection<AccountDebit> Debits { get; set; } = new Collection<AccountDebit>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new Collection<Invoice>();
        public virtual OrganisationModel Organisation { get; set; } = null!;
        public string OrganisationId { get; set; } = null!;
        public double? Balance => GetBalance();

        /// <summary>
        /// The fraction of a purchase taken by Amphora Data.
        /// Is multiplied by the Purchase Amount (i.e. the debit amount) to generate the credit amount.
        /// </summary>
        public double? CommissionRate { get; set; } = DefaultCommissionRate;

        private double GetCommissionRate() => CommissionRate ?? DefaultCommissionRate;

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

        public void DebitAccount(string label, double amount, System.DateTimeOffset? timestamp, string? amphoraId = null)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }

            var currentBalance = Balance;
            if (Debits == null) { Debits = new List<AccountDebit>(); }
            if (amphoraId == null) { Debits.Add(new AccountDebit(label, amount, currentBalance, timestamp)); }
            else { Debits.Add(new AccountDebit(label, amount, currentBalance, timestamp, amphoraId)); }
        }

        public void DebitAccountFromPurchase(PurchaseModel purchase, System.DateTimeOffset? timestamp)
        {
            if (purchase.Price == 0) { return; }
            if (Debits == null) { Debits = new List<AccountDebit>(); }
            var label = $"Purchase: {purchase.Amphora.Name}";
            var currentBalance = Balance;
            Debits.Add(new AccountDebit(label, purchase.Price, currentBalance, timestamp, purchase.AmphoraId));
        }

        public void CreditAccount(string label, double amount, System.DateTimeOffset? timestamp, string? amphoraId = null)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }

            var currentBalance = Balance;
            if (Credits == null) { Credits = new List<AccountCredit>(); }
            if (amphoraId == null) { Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp)); }
            else { Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp, amphoraId)); }
        }

        public void CreditAccountFromSale(PurchaseModel purchase, System.DateTimeOffset? timestamp)
        {
            if (purchase.Price == 0) { return; }
            if (Credits == null) { Credits = new List<AccountCredit>(); }
            var currentBalance = Balance;
            var label = $"Sold {purchase.AmphoraId}";
            var amount = purchase.Price * GetCommissionRate();
            Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp, purchase.AmphoraId));
        }
    }
}