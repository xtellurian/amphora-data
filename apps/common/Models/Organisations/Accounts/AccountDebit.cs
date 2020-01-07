namespace Amphora.Common.Models.Organisations.Accounts
{
    public class AccountDebit : Transaction
    {
        public AccountDebit(string label, double? amount) : base(label, amount)
        {
            IsDebit = true;
        }

        public AccountDebit(string label, double? amount, string amphoraId) : this(label, amount)
        {
            this.AmphoraId = amphoraId;
        }

        public virtual Account Account { get; set; } = null!;
    }
}
