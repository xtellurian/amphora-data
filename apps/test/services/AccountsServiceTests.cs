using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class AccountsServiceTests : UnitTestBase
    {
        private Random rand = new Random();

        [Fact]
        public async Task InvoicesAreGenerated_NullOrg_ThrowsNullRefernce()
        {
            var dtProvider = new MockDateTimeProvider();
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

                var lastMonth = DateTime.Now.AddMonths(-1);
                await Assert.ThrowsAsync<System.ArgumentNullException>(() => sut.GenerateInvoiceAsync(lastMonth, null));
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DebitsAndCreditsThisMonth()
        {
            var dtProvider = new MockDateTimeProvider();
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var otherOrg = EntityLibrary.GetOrganisationModel();
                otherOrg = await orgStore.CreateAsync(otherOrg);
                var credit = rand.Next(1, 105);
                var debit = rand.Next(1, 105);
                org.Account.CreditAccount("Test Credit", credit, dtProvider.UtcNow); // credit 100
                org.Account.DebitAccount("Test Debit", debit, dtProvider.UtcNow); // debit 50

                otherOrg.Account.CreditAccount("Test Credit", debit, dtProvider.UtcNow); // swap
                otherOrg.Account.DebitAccount("Test Debit", credit, dtProvider.UtcNow);

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id);
                var otherInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, otherOrg.Id);

                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(credit - debit, invoice.InvoiceBalance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DebitsAndCreditsMultiMonth()
        {
            var credit = 50;
            var debit = 100;
            var dtProvider = new MockDateTimeProvider();
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var otherOrg = EntityLibrary.GetOrganisationModel();
                otherOrg = await orgStore.CreateAsync(otherOrg);
                var lastMonth = DateTime.Now.AddMonths(-1);
                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // debit 50

                otherOrg.Account.Credits.Add(new AccountCredit("Test Credit", credit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // credit 100
                otherOrg.Account.Debits.Add(new AccountDebit("Test Debit", debit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);
                var thisMonthsInvoice = org.Account.Invoices.FirstOrDefault();
                Assert.NotNull(thisMonthsInvoice);
                Assert.Empty(thisMonthsInvoice.Credits);
                Assert.Empty(thisMonthsInvoice.Debits);
                Assert.Equal(0, thisMonthsInvoice.InvoiceBalance);

                invoice = await sut.GenerateInvoiceAsync(lastMonth.StartOfMonth(), org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(2, org.Account.Invoices.Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(credit - debit, invoice.InvoiceBalance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_RegenerateNotEqual()
        {
            var dtProvider = new MockDateTimeProvider();
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);

                var lastMonth = DateTime.Now.AddMonths(-1);

                var credit = rand.Next(1, 105);
                var debit = rand.Next(1, 105);
                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true);

                Assert.True(invoice.IsPreview);

                var regenereratedInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true, regenerate: true);

                Assert.NotEqual(invoice.Id, regenereratedInvoice.Id);
                Assert.Equal(invoice.InvoiceBalance, regenereratedInvoice.InvoiceBalance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DuplicateReturnsNull()
        {
            var dtProvider = new MockDateTimeProvider();
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);

                var lastMonth = DateTime.Now.AddMonths(-1);

                var credit = rand.Next(1, 105);
                var debit = rand.Next(1, 105);
                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, dtProvider.UtcNow) { CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true);

                Assert.True(invoice.IsPreview);

                var regenereratedInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true, regenerate: false);

                Assert.Null(regenereratedInvoice);
            }
        }
    }
}