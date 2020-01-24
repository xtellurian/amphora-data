namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountDebit : Transaction
    {
        public AccountDebit(string label, double? amount) : base(label, amount)
        {
            IsDebit = true;
        }

        public AccountDebit(string label,
                            double? amount,
                            double? balanceBeforeTransaction,
                            System.DateTimeOffset? timestamp) : base(label, amount, timestamp)
        {
            IsDebit = true;
            this.Balance = balanceBeforeTransaction - amount;
        }

        public AccountDebit(string label,
                            double? amount,
                            double? balanceBeforeTransaction,
                            System.DateTimeOffset? timestamp,
                            string amphoraId) : this(label, amount, balanceBeforeTransaction, timestamp)
        {
            this.AmphoraId = amphoraId;
        }

        public virtual Account Account { get; set; } = null!;
    }
}
