using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Transactions;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Purchasing
{
    public class PurchaseServiceTests : UnitTestBase
    {
        [Fact]
        public async Task PurchasingAmphora_SendsEmail()
        {
            var context = base.GetContext();
            var store = new PurchaseEFStore(context);
            var user_emailconfirmed = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true
            };
            var user_notconfirmed = new ApplicationUser()
            {
                Email = "other@amphoradata.com",
                EmailConfirmed = false
            };

            var amphora = new AmphoraModel();
            amphora.Id = System.Guid.NewGuid().ToString();
            var mockEmailSender = new Mock<IEmailSender>();
            var logger = CreateMockLogger<PurchaseService>();

            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var sut = new PurchaseService(store, null, mockEmailSender.Object, logger);

            var result = await sut.PurchaseAmphora(user_emailconfirmed, amphora);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Entity);
            var result_notconfirmed = await sut.PurchaseAmphora(user_notconfirmed, amphora);
            Assert.True(result_notconfirmed.Succeeded);
            Assert.NotNull(result_notconfirmed.Entity);

            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_emailconfirmed.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user_notconfirmed.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task AcceptedTermsAndConditions_EvaluatesTrue()
        {
            var context = base.GetContext();
            var orgStore = new OrganisationsEFStore(context);
            var amphoraStore = new AmphoraeEFStore(context);
            var store = new PurchaseEFStore(context);

            var terms = new TermsAndConditionsModel
            {
                Name = "1",
                Contents = "This should be accepted"
            };
            var othersOrg = new OrganisationModel()
            {
                TermsAndConditions = new List<TermsAndConditionsModel>
                {
                    terms
                }
            };
            othersOrg = await orgStore.CreateAsync(othersOrg);
            var usersOrg = new OrganisationModel();
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

            var amphora = new AmphoraModel
            {
                OrganisationId = othersOrg.Id,
                TermsAndConditionsId = terms.Name
            };
            amphora = await amphoraStore.CreateAsync(amphora);

            var sut = new PurchaseService(store, null, null, CreateMockLogger<PurchaseService>());
            var result = sut.HasAgreedToTermsAndConditions(user, amphora);

            Assert.True(result);
        }

        [Fact]
        public async Task NotAcceptedTermsAndConditions_EvaluatesFalse()
        {
            var context = base.GetContext();
            var orgStore = new OrganisationsEFStore(context);
            var amphoraStore = new AmphoraeEFStore(context);
            var store = new PurchaseEFStore(context);

            var terms = new TermsAndConditionsModel
            {
                Name = "1",
                Contents = "This shouldn't ber accepted"
            };
            var othersOrg = new OrganisationModel()
            {
                TermsAndConditions = new List<TermsAndConditionsModel>
                {
                    terms
                }
            };
            othersOrg = await orgStore.CreateAsync(othersOrg);
            var usersOrg = new OrganisationModel();
            usersOrg = await orgStore.CreateAsync(usersOrg);
            

            var user = new ApplicationUser()
            {
                Email = "test@amphoradata.com",
                EmailConfirmed = true,
                Organisation = usersOrg
            };

            var amphora = new AmphoraModel
            {
                OrganisationId = othersOrg.Id,
                TermsAndConditionsId = terms.Name
            };
            amphora = await amphoraStore.CreateAsync(amphora);

            var sut = new PurchaseService(store, null, null, CreateMockLogger<PurchaseService>());
            var result = sut.HasAgreedToTermsAndConditions(user, amphora);

            Assert.False(result);
        }
    }
}