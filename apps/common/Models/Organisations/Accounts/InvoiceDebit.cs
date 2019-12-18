namespace Amphora.Common.Models.Organisations.Accounts
{
    public class InvoiceDebit : Transaction
    {
        public InvoiceDebit(string label, double? amount) : base(label, amount)
        {
        }
        
        // public virtual Invoice Invoice { get; set; } = null!;

        public override bool IsCredit => false;

        public override bool IsDebit => true;
    }
}
