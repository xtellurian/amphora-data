namespace Amphora.Common.Models.Organisations
{
    public class Credit
    {
        public Credit() {}
        public Credit(string label, double amount)
        {
            Label = label;
            Amount = amount;
        }
        public double? Amount { get; set; }
        public string Label { get; set; }
    }

}