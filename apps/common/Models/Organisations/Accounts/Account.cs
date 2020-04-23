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
        public virtual Plan? Plan { get; set; } = new Plan();
        public string OrganisationId { get; set; } = null!;
        public double? Balance => GetBalance();

        /// <summary>
        /// Gets or sets the fraction of a purchase taken by Amphora Data.
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
            Debits ??= new Collection<AccountDebit>();
            if (amphoraId == null) { Debits.Add(new AccountDebit(label, amount, currentBalance, timestamp)); }
            else { Debits.Add(new AccountDebit(label, amount, currentBalance, timestamp, amphoraId)); }
        }

        public void DebitAccountFromPurchase(PurchaseModel purchase, System.DateTimeOffset? timestamp)
        {
            Debits ??= new Collection<AccountDebit>();
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
            Credits ??= new List<AccountCredit>();
            if (amphoraId == null) { Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp)); }
            else { Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp, amphoraId)); }
        }

        /// <summary>
        /// Credits an account after a sale.
        /// </summary>
        /// <returns>
        /// Returns the commission amount for the platform.
        /// </returns>
        public double? CreditAccountFromSale(PurchaseModel purchase, System.DateTimeOffset? timestamp)
        {
            Credits ??= new List<AccountCredit>();
            var currentBalance = Balance;
            var label = $"Sold {purchase.AmphoraId}";
            var rawAmount = purchase.Price * GetCommissionRate();
            if (rawAmount.HasValue)
            {
                var amount = System.Math.Round(rawAmount.Value, 2);
                Credits.Add(new AccountCredit(label, amount, currentBalance, timestamp, purchase.AmphoraId));
                return purchase.Price - amount;
            }

            return 0;
        }
    }
}