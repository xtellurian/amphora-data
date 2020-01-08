namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountDebit : Transaction
    {
        public AccountDebit(string label, double? amount, System.DateTimeOffset? timestamp) : base(label, amount, timestamp)
        {
            IsDebit = true;
        }

        public AccountDebit(string label, double? amount, System.DateTimeOffset? timestamp, string amphoraId) : this(label, amount, timestamp)
        {
            this.AmphoraId = amphoraId;
        }

        public virtual Account Account { get; set; } = null!;
    }
}
