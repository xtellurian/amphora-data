namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountCredit : Transaction
    {
        public AccountCredit(string label, double? amount) : base(label, amount)
        {
            IsCredit = true;
        }

        public AccountCredit(string label,
                             double? amount,
                             double? balanceBeforeTransaction,
                             System.DateTimeOffset? timestamp) : base(label, amount, timestamp)
        {
            IsCredit = true;
            this.Balance = balanceBeforeTransaction + amount;
        }

        public AccountCredit(string label,
                             double? amount,
                             double? balanceBeforeTransaction,
                             System.DateTimeOffset? timestamp,
                             string amphoraId) : this(label, amount, balanceBeforeTransaction, timestamp)
        {
            this.AmphoraId = amphoraId;
        }

        public virtual Account? Account { get; set; } = null!;
    }
}
