using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Xunit;

namespace Amphora.Tests.Unit.Purchasing
{
    public class AccountServiceTests : UnitTestBase
    {
        private Random random = new Random();

        [Fact]
        public void PurchaseThisMonth_DeductsFromBalance()
        {
            // TODO
        }

        [Fact]
        public void AccountWithInvoices_CanGetListOfPaid()
        {
            var dtProvider = new MockDateTimeProvider();
            var sut = new Account();
            var invoice = new Invoice();
            invoice.Transactions.Add(new InvoiceTransaction("test1", 5, dtProvider.UtcNow, isCredit: true));
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

        [Fact]
        public async Task WhenPurchasing_InitialDebitIsMatchedByCredit()
        {
            var dtProvider = new MockDateTimeProvider();
            var purchaseStore = new PurchaseEFStore(GetContext(), CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(GetContext(), CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(GetContext(), CreateMockLogger<AmphoraeEFStore>());
            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());
            var userStore = new ApplicationUserStore(GetContext(), CreateMockLogger<ApplicationUserStore>());
            var userService = new UserService(CreateMockLogger<UserService>(), orgStore, userStore, permissionService, null, null);
            var purchaseService = new PurchaseService(purchaseStore,
                                                      orgStore,
                                                      permissionService,
                                                      userService,
                                                      null,
                                                      dtProvider,
                                                      CreateMockLogger<PurchaseService>());

            var sut = new AccountsService(purchaseStore, orgStore, dtProvider, CreateMockLogger<AccountsService>());

            // Create 2 orgs and some amphora
            var sellerOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel("Seller"));
            var buyerOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel("Buyer"));
            var buyer = new ApplicationUser() { Organisation = buyerOrg, OrganisationId = buyerOrg.Id };

            var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(sellerOrg, nameof(WhenPurchasing_InitialDebitIsMatchedByCredit)));

            // purchase that amphora
            var purchaseRes = await purchaseService.PurchaseAmphoraAsync(buyer, amphora);
            Assert.True(purchaseRes.Succeeded);

            // check both buyer and seller have debit and credit respectively
            Assert.Single(buyerOrg.Account.Debits);
            var debit = buyerOrg.Account.Debits.FirstOrDefault();
            Assert.Equal(amphora.Id, debit.AmphoraId);

            MathHelpers.AssertCloseEnough(amphora.Price, debit.Amount);

            Assert.Single(sellerOrg.Account.Credits);
            var credit = sellerOrg.Account.Credits.FirstOrDefault();
            Assert.Equal(amphora.Id, credit.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price * sellerOrg.Account.CommissionRate, credit.Amount);

            // fast fwd one month, and now we should see the recurring debit and credit
            dtProvider.Now = dtProvider.Now.AddMonths(1);
            // run the account service job
            await sut.PopulateDebitsAndCreditsAsync();

            Assert.Equal(2, buyerOrg.Account.Debits.Count);
            var debit2 = buyerOrg.Account.Debits.Where(d => d.Timestamp == dtProvider.Now).FirstOrDefault();
            Assert.NotNull(debit2);
            Assert.Equal(amphora.Id, debit2.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price, debit2.Amount);

            Assert.Equal(2, sellerOrg.Account.Credits.Count);
            var credit2 = sellerOrg.Account.Credits.FirstOrDefault();
            Assert.NotNull(credit2);
            Assert.Equal(amphora.Id, credit2.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price * sellerOrg.Account.CommissionRate, credit2.Amount);
        }
    }
}