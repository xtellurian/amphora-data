namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountCredit : Transaction
    {
        public AccountCredit(string label, double? amount) : base(label, amount)
        {
            IsCredit = true;
        }

        public virtual Account Account { get; set; } = null!;
    }
}
