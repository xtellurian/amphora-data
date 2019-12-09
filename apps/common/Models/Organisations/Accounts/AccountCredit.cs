namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountCredit : Transaction
    {
        public AccountCredit(string label, double? amount) : base(label, amount)
        {
        }

        public virtual Account Account { get; set; } = null!;

        public override bool IsCredit => true;

        public override bool IsDebit => false;
    }
}
