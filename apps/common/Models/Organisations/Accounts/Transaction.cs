using System;

namespace Amphora.Common.Models.Organisations.Accounts
{
    public abstract class Transaction
    {
        public Transaction(string label, double? amount)
        {
            Label = label;
            Amount = amount;
        }
        public string? Id { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;
        public virtual Account Account { get; set; } = null!;
        public double? Amount { get; set; }
        public string Label { get; set; }
        public abstract bool IsCredit { get; }
        public abstract bool IsDebit { get; }
    }
}