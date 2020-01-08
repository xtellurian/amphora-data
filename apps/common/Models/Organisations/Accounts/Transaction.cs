using System;

namespace Amphora.Common.Models.Organisations.Accounts
{
    public abstract class Transaction
    {
        public Transaction(string label, double? amount)
        {
            Label = label;
            Amount = amount.HasValue ? Math.Round(amount.Value, 2) : 0;
        }

        public Transaction(string label, double? amount, DateTimeOffset? timestamp) : this(label, amount)
        {
            Timestamp = timestamp ?? DateTime.UtcNow;
        }

        public string? Id { get; set; }
        public string? AmphoraId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;
        public double? Amount { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string? Label { get; set; }
        public bool? IsCredit { get; set; }
        public bool? IsDebit { get; set; }
    }
}