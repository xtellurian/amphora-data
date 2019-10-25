using System;

namespace Amphora.Common.Models.Organisations
{
    public class Credit
    {
        public Credit() { }
        public Credit(string label, double amount)
        {
            Label = label;
            Amount = amount;
        }
        public string Id { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;
        public virtual Account Account { get; set; }
        public double? Amount { get; set; }
        public string Label { get; set; }
    }

}