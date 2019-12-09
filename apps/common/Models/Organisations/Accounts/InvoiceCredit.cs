namespace Amphora.Common.Models.Organisations.Accounts
{
    public class InvoiceCredit : Transaction
    {
        public InvoiceCredit(string label, double? amount) : base(label, amount)
        {
        }

        public virtual Invoice Invoice { get; set; } = null!;

        public override bool IsCredit => true;

        public override bool IsDebit => false;
    }
}
