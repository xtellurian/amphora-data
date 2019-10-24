using System.Collections.Generic;
using System.Linq;

namespace Amphora.Common.Models.Organisations
{
    public class Account
    {
        public virtual ICollection<Credit> Credits { get; set; }
        public virtual ICollection<Debit> Debits { get; set; }
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
            if (Debits == null) Debits = new List<Debit>();
            Debits.Add(new Debit(label, amount));
        }
        public void CreditAccount(string label, double amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentException("Amount < 0");
            }
            if (Credits == null) Credits = new List<Credit>();
            Credits.Add(new Credit(label, amount));
        }
    }
}