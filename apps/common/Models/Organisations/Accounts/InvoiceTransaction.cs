namespace Amphora.Common.Models.Organisations.Accounts
{
    public class InvoiceTransaction : Transaction
    {
        protected InvoiceTransaction(string label, double? amount) : base(label, amount) { }

        public InvoiceTransaction(string label, double? amount, bool? isCredit = null, bool? isDebit = null) : this(label, amount)
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
        }
    }
}
