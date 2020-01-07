using Amphora.Common.Models.Organisations.Accounts;
using Xunit;

namespace Amphora.Tests.Unit.Purchasing
{
    public class AccountServiceTests
    {
        [Fact]
        public void PurchaseThisMonth_DeductsFromBalance()
        {
            // TODO
        }

        [Fact]
        public void AccountWithInvoices_CanGetListOfPaid()
        {
            var sut = new Account();
            var invoice = new Invoice();
            invoice.Transactions.Add(new InvoiceTransaction("test1", 5, isCredit: true));
            invoice.IsPaid = false;
            invoice.IsPreview = false;
            sut.Invoices.Add(invoice);

            var unpaid = sut.GetUnpaidInvoices();
            var paid = sut.GetPaidInvoices();

            Assert.Single(unpaid);
            Assert.Empty(paid);

            invoice.IsPaid = true;

            unpaid = sut.GetUnpaidInvoices();
            paid = sut.GetPaidInvoices();
            // swapped
            Assert.Single(paid);
            Assert.Empty(unpaid);
        }
    }
}