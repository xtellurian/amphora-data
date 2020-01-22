namespace Amphora.Common.Models.Organisations.Accounts
{
    public class InvoiceTransaction : Transaction
    {
        public InvoiceTransaction(string label, double? amount) : base(label, amount) { }
        protected InvoiceTransaction(string? label,
                                     double? amount,
                                     System.DateTimeOffset? timestamp) : base(label, amount, timestamp) { }

        public InvoiceTransaction(Transaction transaction) : this(transaction.Label, transaction.Amount, transaction.Timestamp)
        {
            if (transaction is null)
            {
                throw new System.ArgumentNullException(nameof(transaction));
            }

            if (transaction.IsDebit == true)
            {
                IsDebit = true;
            }
            else if (transaction.IsCredit == true)
            {
                IsCredit = true;
            }

            this.Balance = transaction.Balance;
            this.AmphoraId = transaction.AmphoraId;
        }

        public InvoiceTransaction(string label,
                                  double? amount,
                                  double? balanceBeforeTransaction,
                                  System.DateTimeOffset? timestamp,
                                  bool? isCredit = null,
                                  bool? isDebit = null) : this(label, amount, timestamp)
        {
            if (isCredit.HasValue && isCredit.Value && isDebit.HasValue && isDebit.Value)
            {
                throw new System.ArgumentException("IsCredit and IsDebit are both true.");
            }

            if (isCredit.HasValue && isCredit.Value)
            {
                this.IsCredit = true;
            }
            else if (isDebit.HasValue && isDebit.Value)
            {
                this.IsDebit = true;
            }
            else if (amount.HasValue && amount.Value > 0)
            {
                this.IsCredit = true;
            }
            else if (amount.HasValue && amount.Value <= 0)
            {
                this.IsDebit = true;
            }
            else
            {
                throw new System.ArgumentException("IsCredit and IsDebit are both false and amount is null.");
            }

            if (this.IsCredit == true)
            {
                this.Balance = balanceBeforeTransaction + amount;
            }
            else if (this.IsDebit == true)
            {
                this.Balance = balanceBeforeTransaction - amount;
            }
        }
    }
}
