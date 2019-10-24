namespace Amphora.Common.Models.Organisations
{
    public class Debit
    {
        public Debit() { }
        public Debit(string label, double amount)
        {
            Label = label;
            Amount = amount;
        }
        public string Id { get; set; }
        public virtual Account Account { get; set; }
        public double? Amount { get; set; }
        public string Label { get; set; }
    }
}