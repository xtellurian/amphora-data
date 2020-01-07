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
        public string? AmphoraId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;
        public double? Amount { get; set; }
        public string Label { get; set; }
        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
    }
}