using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Purchasing
{
    public class PurchaseServiceTests : UnitTestBase
    {
        [Fact]
        public async Task PurchasingAmphora_SendsEmail()
        {
            var context = GetContext();
            var dtProvider = new MockDateTimeProvider();
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var org = EntityLibrary.GetOrganisationModel();
            var otherOrg = EntityLibrary.GetOrganisationModel();
            var thirdOrg = EntityLibrary.GetOrganisationModel();
            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());

            org = await orgStore.CreateAsync(org);
            otherOrg = await orgStore.CreateAsync(otherOrg);
            thirdOrg = await orgStore.CreateAsync(thirdOrg);
            var user_emailconfirmed = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true,
                OrganisationId = org.Id
            };
            var user_notconfirmed = new ApplicationUser()
            {
                Email = "other@amphoradata.com",
                EmailConfirmed = false,
                OrganisationId = thirdOrg.Id
            };
            var user_noOrg = new ApplicationUser()
            {
                Email = "no-org@amphoradata.com",
                EmailConfirmed = false
            };

            var amphora = EntityLibrary.GetAmphoraModel(otherOrg, nameof(PurchasingAmphora_SendsEmail));
            amphora.Id = System.Guid.NewGuid().ToString();
            var mockEmailSender = new Mock<IEmailSender>();
            var logger = CreateMockLogger<PurchaseService>();

            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var sut = new PurchaseService(store, orgStore, permissionService,  null, mockEmailSender.Object, dtProvider, logger);

            var result = await sut.PurchaseAmphoraAsync(user_emailconfirmed, amphora);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Entity);
            Assert.NotNull(result.Entity.PurchasedByOrganisationId);

            var result_notconfirmed = await sut.PurchaseAmphoraAsync(user_notconfirmed, amphora);
            Assert.True(result_notconfirmed.Succeeded);
            Assert.NotNull(result_notconfirmed.Entity);
            Assert.NotNull(result_notconfirmed.Entity.PurchasedByOrganisationId);

            var result_noOrg = await sut.PurchaseAmphoraAsync(user_noOrg, amphora);
            Assert.False(result_noOrg.Succeeded);
            Assert.Null(result_noOrg.Entity);
            Assert.False(result_noOrg.Succeeded);

            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_emailconfirmed.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_notconfirmed.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_noOrg.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task AcceptedTermsAndConditions_EvaluatesTrue()
        {
            var dtProvider = new MockDateTimeProvider();
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());

            var terms = new TermsAndConditionsModel("1", System.Guid.NewGuid().ToString(), "This should be accepted");

            var othersOrg = EntityLibrary.GetOrganisationModel();

            othersOrg.TermsAndConditions = new List<TermsAndConditionsModel>
                {
                    terms
                };

            othersOrg = await orgStore.CreateAsync(othersOrg);
            var usersOrg = EntityLibrary.GetOrganisationModel();
            usersOrg.TermsAndConditionsAccepted = new List<TermsAndConditionsAcceptanceModel>
            {
                new TermsAndConditionsAcceptanceModel(usersOrg, terms)
            };
            usersOrg = await orgStore.CreateAsync(usersOrg);

            var user = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true,
                Organisation = usersOrg
            };

            var amphora = EntityLibrary.GetAmphoraModel(othersOrg, nameof(AcceptedTermsAndConditions_EvaluatesTrue));
            amphora.TermsAndConditionsId = terms.Id;

            amphora = await amphoraStore.CreateAsync(amphora);

            var sut = new PurchaseService(store, orgStore, permissionService, null, null, dtProvider, CreateMockLogger<PurchaseService>());
            var result = sut.HasAgreedToTermsAndConditions(user, amphora);

            Assert.True(result);
        }

        [Fact]
        public async Task NotAcceptedTermsAndConditions_EvaluatesFalse()
        {
            var context = GetContext();
            var dtProvider = new MockDateTimeProvider();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());

            var terms = new TermsAndConditionsModel("2", "FooBar", "TThis shouldn't be accepted");

            var othersOrg = EntityLibrary.GetOrganisationModel();

            othersOrg.TermsAndConditions = new List<TermsAndConditionsModel>
                {
                    terms
                };

            othersOrg = await orgStore.CreateAsync(othersOrg);
            var usersOrg = EntityLibrary.GetOrganisationModel();
            usersOrg = await orgStore.CreateAsync(usersOrg);

            var user = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true,
                Organisation = usersOrg
            };

            var amphora = EntityLibrary.GetAmphoraModel(othersOrg, nameof(NotAcceptedTermsAndConditions_EvaluatesFalse));
            amphora.TermsAndConditionsId = terms.Id;

            amphora = await amphoraStore.CreateAsync(amphora);

            var sut = new PurchaseService(store, orgStore, permissionService, null, null, dtProvider, CreateMockLogger<PurchaseService>());
            var result = sut.HasAgreedToTermsAndConditions(user, amphora);

            Assert.False(result);
        }

        [Fact]
        public async Task PurchaseAmphora_DeductsBalance()
        {
            var dtProvider = new MockDateTimeProvider();
            var context = GetContext();
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());

            var org = EntityLibrary.GetOrganisationModel();
            org.Account = new Account()
            {
                Credits = new List<AccountCredit>()
                    {
                        new AccountCredit("initial", 100, dtProvider.UtcNow)
                    }
            };

            var otherOrg = EntityLibrary.GetOrganisationModel();

            org = await orgStore.CreateAsync(org);
            otherOrg = await orgStore.CreateAsync(otherOrg);
            var amphora = EntityLibrary.GetAmphoraModel(otherOrg, nameof(NotAcceptedTermsAndConditions_EvaluatesFalse), true);
            amphora.Price = 9;
            amphora = await amphoraStore.CreateAsync(amphora);

            var user = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true,
                OrganisationId = org.Id
            };

            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var sut = new PurchaseService(store, orgStore, permissionService, null, mockEmailSender.Object, dtProvider, CreateMockLogger<PurchaseService>());
            var purchase = await sut.PurchaseAmphoraAsync(user, amphora);
            Assert.True(purchase.Succeeded);
            org = await orgStore.ReadAsync(org.Id); // reload, just in case
            Assert.Equal(91, org.Account.Balance);
        }
    }
}