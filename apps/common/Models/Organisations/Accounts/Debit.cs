namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Debit : Transaction
    {
        public Debit(string label, double? amount) : base(label, amount)
        {
        }

        public override bool IsCredit => false;

        public override bool IsDebit => true;
    }
}
