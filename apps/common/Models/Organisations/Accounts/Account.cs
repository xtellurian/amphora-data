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

        public double? GetBalance()
        {
            var credit = this.Credits?.Sum(c => c.Amount) ?? 0;
            var debit = this.Debits?.Sum(d => d.Amount) ?? 0;
            return credit - debit;
        }
        public void DebitAccount(string label, double amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }
            if (Debits == null) Debits = new List<AccountDebit>();
            Debits.Add(new AccountDebit(label, amount));
        }
        public void CreditAccount(string label, double amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }
            if (Credits == null) Credits = new List<AccountCredit>();
            Credits.Add(new AccountCredit(label, amount));
        }
    }
}