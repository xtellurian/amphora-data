using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Common.Services.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
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
            var userDataStore = new ApplicationUserDataEFStore(GetContext(), CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);
            var org = EntityLibrary.GetOrganisationModel();
            var otherOrg = EntityLibrary.GetOrganisationModel();
            var thirdOrg = EntityLibrary.GetOrganisationModel();
            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());

            org = await orgStore.CreateAsync(org);
            otherOrg = await orgStore.CreateAsync(otherOrg);
            thirdOrg = await orgStore.CreateAsync(thirdOrg);
            var user_emailconfirmed = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "test@amphoradata.com",
                    EmailConfirmed = true,
                },
                OrganisationId = org.Id
            };
            var user_notconfirmed = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "other@amphoradata.com",
                    EmailConfirmed = false,
                },
                OrganisationId = thirdOrg.Id
            };
            var user_noOrg = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "no-org@amphoradata.com",
                    EmailConfirmed = false
                },
            };

            var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(otherOrg));
            var mockEmailSender = new Mock<IEmailSender>();
            var logger = CreateMockLogger<PurchaseService>();

            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));

            var sut = new PurchaseService(store, orgStore, permissionService, commissionMock.Object, null, mockEmailSender.Object, CreateMockEventPublisher(), dtProvider, logger);

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

            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_emailconfirmed.ContactInformation.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_notconfirmed.ContactInformation.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_noOrg.ContactInformation.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task AcceptedTermsAndConditions_EvaluatesTrue()
        {
            var dtProvider = new MockDateTimeProvider();
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var userDataStore = new ApplicationUserDataEFStore(GetContext(), CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);
            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());

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

            var user = new ApplicationUserDataModel()
            {
                ContactInformation = new ContactInformation
                {
                    Email = "test@amphoradata.com",
                    EmailConfirmed = true,
                },
                Organisation = usersOrg
            };

            var amphora = EntityLibrary.GetAmphoraModel(othersOrg);
            amphora.TermsAndConditionsId = terms.Id;

            amphora = await amphoraStore.CreateAsync(amphora);
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));

            var sut = new PurchaseService(store, orgStore, permissionService, commissionMock.Object, null, null, CreateMockEventPublisher(), dtProvider, CreateMockLogger<PurchaseService>());
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
            var userDataStore = new ApplicationUserDataEFStore(GetContext(), CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);
            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());

            var terms = new TermsAndConditionsModel("2", "FooBar", "TThis shouldn't be accepted");

            var othersOrg = EntityLibrary.GetOrganisationModel();

            othersOrg.TermsAndConditions = new List<TermsAndConditionsModel>
                {
                    terms
                };

            othersOrg = await orgStore.CreateAsync(othersOrg);
            var usersOrg = EntityLibrary.GetOrganisationModel();
            usersOrg = await orgStore.CreateAsync(usersOrg);

            var user = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "test@amphoradata.com",
                    EmailConfirmed = true,
                },
                Organisation = usersOrg
            };

            var amphora = EntityLibrary.GetAmphoraModel(othersOrg);
            amphora.TermsAndConditionsId = terms.Id;

            amphora = await amphoraStore.CreateAsync(amphora);
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));

            var sut = new PurchaseService(store, orgStore, permissionService, commissionMock.Object, null, null, CreateMockEventPublisher(), dtProvider, CreateMockLogger<PurchaseService>());
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
            var userDataStore = new ApplicationUserDataEFStore(GetContext(), CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);
            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());

            var org = EntityLibrary.GetOrganisationModel();
            org.Account = new Account()
            {
                Credits = new List<AccountCredit>()
                    {
                        new AccountCredit("initial", 100, -20, dtProvider.UtcNow)
                    }
            };

            var otherOrg = EntityLibrary.GetOrganisationModel();

            org = await orgStore.CreateAsync(org);
            otherOrg = await orgStore.CreateAsync(otherOrg);
            var amphora = EntityLibrary.GetAmphoraModel(otherOrg, true);
            amphora.Price = 9;
            amphora = await amphoraStore.CreateAsync(amphora);

            var user = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "test@amphoradata.com",
                    EmailConfirmed = true,
                },
                OrganisationId = org.Id
            };

            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));

            var sut = new PurchaseService(store, orgStore, permissionService, commissionMock.Object, null, mockEmailSender.Object, CreateMockEventPublisher(), dtProvider, CreateMockLogger<PurchaseService>());
            var purchase = await sut.PurchaseAmphoraAsync(user, amphora);
            Assert.True(purchase.Succeeded);
            org = await orgStore.ReadAsync(org.Id); // reload, just in case
            Assert.Equal(91, org.Account.Balance);
        }

        [Fact]
        public async Task PurchaseZeroDollarAmphora_ShowsAsDebit()
        {
            var dtProvider = new MockDateTimeProvider();
            var context = GetContext();
            var store = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var userDataStore = new ApplicationUserDataEFStore(GetContext(), CreateMockLogger<ApplicationUserDataEFStore>());
            var userDataService = new ApplicationUserDataService(userDataStore);
            var permissionService = new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());

            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var otherOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());

            var amphora = EntityLibrary.GetAmphoraModel(otherOrg, true);
            amphora.Price = 0;
            amphora = await amphoraStore.CreateAsync(amphora);

            var user = new ApplicationUserDataModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                ContactInformation = new ContactInformation
                {
                    Email = "test@amphoradata.com",
                    EmailConfirmed = true,
                },
                OrganisationId = org.Id
            };

            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var commissionMock = new Mock<ICommissionTrackingService>();
            commissionMock.Setup(mock => mock.TrackCommissionAsync(It.IsAny<PurchaseModel>(), It.IsAny<double?>()));

            var sut = new PurchaseService(store, orgStore, permissionService, commissionMock.Object, null, mockEmailSender.Object, CreateMockEventPublisher(), dtProvider, CreateMockLogger<PurchaseService>());
            var purchase = await sut.PurchaseAmphoraAsync(user, amphora);
            Assert.True(purchase.Succeeded);
            org = await orgStore.ReadAsync(org.Id); // reload, just in case
            Assert.Equal(0, org.Account.Balance);
            Assert.Equal(1, org.Account.Debits.Count);
        }
    }
}