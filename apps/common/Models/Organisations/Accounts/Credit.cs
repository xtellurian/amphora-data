namespace Amphora.Common.Models.Organisations.Accounts
{
    public class Credit : Transaction
    {
        public Credit(string label, double? amount) : base(label, amount)
        {
        }

        public override bool IsCredit => true;

        public override bool IsDebit => false;
    }
}
