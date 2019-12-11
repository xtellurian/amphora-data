namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountDebit : Transaction
    {
        public AccountDebit(string label, double? amount) : base(label, amount)
        {
        }
        public virtual Account Account { get; set; } = null!;

        public override bool IsCredit => false;

        public override bool IsDebit => true;
    }
}
