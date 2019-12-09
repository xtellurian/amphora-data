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
        [Fact]
        public async Task InvoicesAreGenerated_Empty()
        {
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, CreateMockLogger<AccountsService>());


                var lastMonth = DateTime.Now.AddMonths(-1);
                var invoices = await sut.GenerateInvoicesAsync(lastMonth);

                Assert.Empty(invoices);
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

                org.Account.CreditAccount("Test Credit", 100); // credit 100
                org.Account.DebitAccount("Test Debit", 50);// debit 50

                otherOrg.Account.CreditAccount("Test Credit", 50); // credit 50
                otherOrg.Account.DebitAccount("Test Debit", 100);// debit 100

                var invoices = await sut.GenerateInvoicesAsync(DateTime.Now);
                Assert.Equal(2, invoices.Count()); // 2 orgs, 2 invoices
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);

                var orgInvoice = invoices.FirstOrDefault(_ => _.Account.OrganisationId == org.Id);

                Assert.NotNull(orgInvoice);

                Assert.Single(orgInvoice.Credits);
                Assert.Single(orgInvoice.Debits);
                Assert.Equal(50, orgInvoice.Balance);
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

                var invoices = await sut.GenerateInvoicesAsync(DateTime.Now);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices.Count);
                var thisMonthsInvoice = org.Account.Invoices.FirstOrDefault();
                Assert.NotNull(thisMonthsInvoice);
                Assert.Empty(thisMonthsInvoice.Credits);
                Assert.Empty(thisMonthsInvoice.Debits);

                invoices = await sut.GenerateInvoicesAsync(lastMonth.StartOfMonth());
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(2, org.Account.Invoices.Count);

                var orgInvoice = invoices.FirstOrDefault(_ => _.Account.OrganisationId == org.Id);

                Assert.NotNull(orgInvoice);

                Assert.Single(orgInvoice.Credits);
                Assert.Single(orgInvoice.Debits);
                Assert.Equal(50, orgInvoice.Balance);
            }
        }
    }
}