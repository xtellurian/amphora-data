using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Common.Services.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Moq;
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
                var commissionMock = new Mock<ICommissionTrackingService>();
                commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
                var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

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
                var commissionMock = new Mock<ICommissionTrackingService>();
                commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
                var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

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

                Assert.Equal(1, org.Account.Invoices().Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(credit - debit, invoice.InvoiceBalance);
                // this is only called when we run the PopulateDebitsAndCreditsAsync()
                commissionMock.Verify(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double>()), Times.Never());
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DebitsAndCreditsMultiMonth()
        {
            var credit = 50;
            var debit = 100;
            var dtProvider = new MockDateTimeProvider();
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var otherOrg = EntityLibrary.GetOrganisationModel();
                otherOrg = await orgStore.CreateAsync(otherOrg);
                var lastMonth = DateTime.Now.AddMonths(-1);
                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, org.Account.Balance, dtProvider.UtcNow) { Timestamp = lastMonth }); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, org.Account.Balance, dtProvider.UtcNow) { Timestamp = lastMonth }); // debit 50

                otherOrg.Account.Credits.Add(new AccountCredit("Test Credit", credit, otherOrg.Account.Balance, dtProvider.UtcNow) { Timestamp = lastMonth }); // credit 100
                otherOrg.Account.Debits.Add(new AccountDebit("Test Debit", debit, otherOrg.Account.Balance, dtProvider.UtcNow) { Timestamp = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(1, org.Account.Invoices().Count);
                var thisMonthsInvoice = org.Account.Invoices().FirstOrDefault();
                Assert.NotNull(thisMonthsInvoice);
                Assert.Empty(thisMonthsInvoice.Credits);
                Assert.Empty(thisMonthsInvoice.Debits);
                Assert.Equal(0, thisMonthsInvoice.InvoiceBalance);

                invoice = await sut.GenerateInvoiceAsync(lastMonth.StartOfMonth(), org.Id);
                org = await orgStore.ReadAsync(org.Id); // update org from "db"

                Assert.Equal(2, org.Account.Invoices().Count);

                Assert.NotNull(invoice);

                Assert.Single(invoice.Credits);
                Assert.Single(invoice.Debits);
                Assert.Equal(credit - debit, invoice.InvoiceBalance);
                commissionMock.Verify(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double>()), Times.Never());
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_RegenerateNotEqual()
        {
            var transactionTimestamp = DateTime.Now;
            if (transactionTimestamp.Day > 25)
            {
                transactionTimestamp = transactionTimestamp.AddDays(-5);
            }
            else if (transactionTimestamp.Day < 5)
            {
                transactionTimestamp = transactionTimestamp.AddDays(5);
            }

            var dtProvider = new MockDateTimeProvider(transactionTimestamp);
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);

                var credit = rand.Next(1, 105);
                var debit = rand.Next(1, 105);
                while (debit == credit)
                {
                    // ensure debit and credit are not equal
                    debit = rand.Next(1, 105);
                }

                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, org.Account.Balance, transactionTimestamp)); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, org.Account.Balance, transactionTimestamp)); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(transactionTimestamp, org.Id, isPreview: true);
                Assert.True(invoice.IsPreview);
                invoice.Credits.Should().NotBeNullOrEmpty();
                invoice.Debits.Should().NotBeNullOrEmpty();

                var regenereratedInvoice = await sut.GenerateInvoiceAsync(transactionTimestamp, org.Id, isPreview: true, regenerate: true);
                regenereratedInvoice.Credits.Should().NotBeNullOrEmpty();
                regenereratedInvoice.Debits.Should().NotBeNullOrEmpty();
                Assert.NotEqual(invoice.Id, regenereratedInvoice.Id);
                Assert.Equal(invoice.InvoiceBalance, regenereratedInvoice.InvoiceBalance);
                commissionMock.Verify(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double>()), Times.Never());
                var oldBalance = org.Account.Balance;
                var lastInvoice = await sut.GenerateInvoiceAsync(transactionTimestamp, org.Id, isPreview: false, regenerate: true);
                lastInvoice.Credits.Should().NotBeNullOrEmpty();
                lastInvoice.Debits.Should().NotBeNullOrEmpty();
                var newBalance = org.Account.Balance;
                Assert.Equal(oldBalance, newBalance);
            }
        }

        [Fact]
        public async Task InvoicesAreGenerated_DuplicateReturnsExisting()
        {
            var dtProvider = new MockDateTimeProvider();
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
            using (var context = GetContext())
            {
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);

                var lastMonth = DateTime.Now.AddMonths(-1);

                var credit = rand.Next(1, 105);
                var debit = rand.Next(1, 105);
                org.Account.Credits.Add(new AccountCredit("Test Credit", credit, org.Account.Balance, dtProvider.UtcNow) { CreatedDate = lastMonth }); // credit 100
                org.Account.Debits.Add(new AccountDebit("Test Debit", debit, org.Account.Balance, dtProvider.UtcNow) { CreatedDate = lastMonth }); // debit 50

                var invoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true);

                Assert.True(invoice.IsPreview);

                var oldInvoice = await sut.GenerateInvoiceAsync(DateTime.Now, org.Id, isPreview: true, regenerate: false);

                oldInvoice.Should().NotBeNull();
                oldInvoice.Id.Should().Be(invoice.Id);
                commissionMock.Verify(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double>()), Times.Never());
            }
        }

        [Fact]
        public void AccountWithInvoices_CanGetListOfPaid()
        {
            var dtProvider = new MockDateTimeProvider();
            var sut = new Account
            {
                Organisation = new OrganisationModel
                {
                    Invoices = new Collection<InvoiceModel>()
                }
            };
            var invoice = new InvoiceModel();
            invoice.Transactions.Add(new InvoiceTransaction("test1", 5, 10, dtProvider.UtcNow, isCredit: true));
            invoice.IsPaid = false;
            invoice.IsPreview = false;
            sut.Invoices().Add(invoice);

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
        public async Task AccountWithInvoice_InvoiceBalanceMatchesAccountBalance()
        {
            var context = GetContext();
            var dtProvider = new MockDateTimeProvider();
            var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
            var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

            // create a test org
            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            double creditAmount = rand.NextDouble() * rand.Next(1, 150);
            double debitAmount = rand.NextDouble() * rand.Next(1, 150);

            org.Account.CreditAccount("test credit", creditAmount, dtProvider.Now);
            org.Account.CreditAccount("test debit", debitAmount, dtProvider.Now.AddSeconds(5));

            var invoice = await sut.GenerateInvoiceAsync(dtProvider.Now, org.Id);

            Assert.Equal(2, invoice.Transactions.Count);
            double? balance = 0;
            foreach (var t in invoice.Transactions)
            {
                Assert.NotNull(t.Balance);
                balance += t.IsCredit == true ? t.Amount : -t.Amount;
                MathHelpers.AssertCloseEnough(balance, t.Balance);
            }

            Assert.Equal(balance, org.Account.Balance);
        }

        [Fact]
        public async Task WhenPurchasing_InitialDebitIsMatchedByCredit()
        {
            var context = GetContext();
            var dtProvider = new MockDateTimeProvider();
            var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var userDataStore = new ApplicationUserDataEFStore(context, CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);

            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());
            var commissionMock = new Mock<ICommissionTrackingService>();
            var mockEmailSender = new Mock<IEmailSender>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));
            var purchaseService = new PurchaseService(purchaseStore,
                                                      orgStore,
                                                      permissionService,
                                                      commissionMock.Object,
                                                      userDataService,
                                                      mockEmailSender.Object,
                                                      CreateMockEventPublisher(),
                                                      dtProvider,
                                                      CreateMockLogger<PurchaseService>());
            var sut = new AccountsService(purchaseStore, orgStore, commissionMock.Object, dtProvider, CreateMockLogger<AccountsService>());

            // Create 2 orgs and some amphora
            var sellerOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel("Seller"));
            var buyerOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel("Buyer"));
            var buyer = new ApplicationUserDataModel
            {
                ContactInformation = new ContactInformation
                {
                    Email = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                },
                Id = System.Guid.NewGuid().ToString(),
                Organisation = buyerOrg,
                OrganisationId = buyerOrg.Id
            };
            buyer = await userDataStore.CreateAsync(buyer);
            var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(sellerOrg));
            var deleted_amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(sellerOrg));

            // purchase that amphora
            var purchaseRes = await purchaseService.PurchaseAmphoraAsync(buyer, amphora);
            Assert.True(purchaseRes.Succeeded);
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            // check both buyer and seller have debit and credit respectively
            Assert.Single(buyerOrg.Account.Debits);
            var debit = buyerOrg.Account.Debits.FirstOrDefault();
            Assert.Equal(amphora.Id, debit.AmphoraId);

            MathHelpers.AssertCloseEnough(amphora.Price, debit.Amount);

            Assert.Single(sellerOrg.Account.Credits);
            var credit = sellerOrg.Account.Credits.FirstOrDefault();
            Assert.Equal(amphora.Id, credit.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price * sellerOrg.Account.CommissionRate, credit.Amount);

            // also purchase the amphora that will be deleted, but don't bother with all assertions
            var purchased_deleted_op = await purchaseService.PurchaseAmphoraAsync(buyer, deleted_amphora);
            Assert.True(purchased_deleted_op.Succeeded);

            // fast fwd one month, and now we should see the recurring debit and credit
            dtProvider.SetFixed(dtProvider.Now.AddMonths(1));

            // delete one just before running the thing
            deleted_amphora.ttl = 3600;
            deleted_amphora.IsDeleted = true;
            await amphoraStore.UpdateAsync(deleted_amphora);

            // run the account service job
            var report = await sut.PopulateDebitsAndCreditsAsync();

            var deleted_purchase = await purchaseStore.ReadAsync(purchased_deleted_op.Entity.Id);
            Assert.Null(deleted_purchase);

            // check the report has messages
            Assert.NotNull(report);
            Assert.NotEmpty(report.LogMessages);
            Assert.Equal(1, report.LogMessages.LongCount(_ => _.Level == Api.Models.Dtos.Admin.Report.Level.Warning));
            Assert.Equal(1, report.LogMessages.LongCount(_ => _.Level == Api.Models.Dtos.Admin.Report.Level.Information));

            Assert.Equal(3, buyerOrg.Account.Debits.Count);
            var debit2 = buyerOrg.Account.Debits.Where(d => d.Timestamp == dtProvider.UtcNow).FirstOrDefault();
            Assert.NotNull(debit2);
            Assert.Equal(amphora.Id, debit2.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price, debit2.Amount);

            Assert.Equal(3, sellerOrg.Account.Credits.Count);
            var credit2 = sellerOrg.Account.Credits.FirstOrDefault();
            Assert.NotNull(credit2);
            Assert.Equal(amphora.Id, credit2.AmphoraId);
            MathHelpers.AssertCloseEnough(amphora.Price * sellerOrg.Account.CommissionRate, credit2.Amount);

            // check we earned commission
            commissionMock.Verify(mock => mock.TrackCommissionAsync(purchaseRes.Entity,
                It.IsInRange<double>(0, purchaseRes.Entity.Price.Value, Moq.Range.Exclusive)), Times.Exactly(2));
        }
    }
}