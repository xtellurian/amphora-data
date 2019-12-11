using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Extensions;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class AccountsServiceTests : UnitTestBase
    {
        Random rand = new Random();

        [Fact]
        public async Task InvoicesAreGenerated_NullOrg_ThrowsNullRefernce()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());


                var lastMonth = DateTime.Now.AddMonths(-1);
                await Assert.ThrowsAsync<System.ArgumentNullException>( () => sut.GenerateInvoiceAsync(lastMonth, null));
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DebitsAndCreditsThisMonth()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var otherOrg = EntityLibrary.GetOrganisationModel();
                otherOrg = await orgStore.CreateAsync(otherOrg);
                var credit = rand.Next(1,105);
                var debit = rand.Next(1,105);
                org.Account.CreditAccount("Test Credit", credit); // credit 100
                org.Account.DebitAccount("Test Debit", debit);// debit 50

                otherOrg.Account.CreditAccount("Test Credit", debit); // swap
                otherOrg.Account.DebitAccount("Test Debit", credit);

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id);
                var otherInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, otherOrg.Id);

                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(credit - debit, invoice.Balance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DebitsAndCreditsMultiMonth()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var otherOrg = EntityLibrary.GetOrganisationModel();
                otherOrg = await orgStore.CreateAsync(otherOrg);
                var lastMonth = DateTime.Now.AddMonths(-1);
                org.Account.Credits.Add( new AccountCredit("Test Credit", 100){ CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add( new AccountDebit("Test Debit", 50){ CreatedDate = lastMonth }); // debit 50

                otherOrg.Account.Credits.Add( new AccountCredit("Test Credit", 50){ CreatedDate = lastMonth }); // credit 100
                otherOrg.Account.Debits.Add( new AccountDebit("Test Debit", 100){ CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);
                var thisMonthsInvoice = org.Account.Invoices.FirstOrDefault();
                Assert.NotNull(thisMonthsInvoice);
                Assert.Empty(thisMonthsInvoice.Credits);
                Assert.Empty(thisMonthsInvoice.Debits);

                invoice = await sut.GenerateInvoiceAsync(lastMonth.StartOfMonth(), org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(2, org.Account.Invoices.Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(50, invoice.Balance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_RegenerateNotEqual()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                
                var lastMonth = DateTime.Now.AddMonths(-1);
                
                var credit = rand.Next(1,105);
                var debit = rand.Next(1,105);
                org.Account.Credits.Add( new AccountCredit("Test Credit", credit){ CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add( new AccountDebit("Test Debit", debit){ CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true);

                Assert.True(invoice.IsPreview);

                var regenereratedInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true, regenerate: true);

                Assert.NotEqual(invoice.Id, regenereratedInvoice.Id);
                Assert.Equal(invoice.Balance, regenereratedInvoice.Balance);

            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DuplicateReturnsNull()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                
                var lastMonth = DateTime.Now.AddMonths(-1);
                
                var credit = rand.Next(1,105);
                var debit = rand.Next(1,105);
                org.Account.Credits.Add( new AccountCredit("Test Credit", credit){ CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add( new AccountDebit("Test Debit", debit){ CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true);

                Assert.True(invoice.IsPreview);

                var regenereratedInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true, regenerate: false);

                Assert.Null(regenereratedInvoice);
            }
        }
    }
}