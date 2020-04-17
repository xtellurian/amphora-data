using System.Threading.Tasks;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Emails;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Permissions.Rules;
using Amphora.Common.Models.Users;
using Amphora.Common.Services.Access;
using Amphora.Tests.Helpers;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class AccessControlServiceTests : UnitTestBase
    {
        [Fact]
        public async Task AdminOfAmphora_CanCreateAndDeleteUserAccessRules()
        {
            var mockPrincipal = MockClaimsPrincipal();
            var context = GetContext();
            var ruleStore = new AmphoraAccessControlsEFStore(context, CreateMockLogger<AmphoraAccessControlsEFStore>());
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org);
            var thisUser = new ApplicationUserDataModel("thisUser", "thisUser", null, null);
            var userToDeny = new ApplicationUserDataModel("otherUser", "otherUser", null, null);
            var mockUserDataService = new Mock<IUserDataService>();
            mockUserDataService.Setup(_ => _.ReadAsync(mockPrincipal.Object, null))
                .ReturnsAsync(new Common.Models.EntityOperationResult<ApplicationUserDataModel>(thisUser, thisUser));
            var mockPermissionService = new Mock<IPermissionService>();

            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(
                    It.IsAny<IUser>(),
                    amphora,
                    It.IsInRange(AccessLevels.Read, AccessLevels.Administer, Range.Inclusive)))
                .ReturnsAsync(true);

            var mockEmailSender = new Mock<IEmailSender>();

            var sut = new AccessControlService(ruleStore,
                                               mockUserDataService.Object,
                                               mockPermissionService.Object,
                                               mockEmailSender.Object);

            var newRule = new UserAccessRule(Kind.Deny, 100, userToDeny);
            var createRes = await sut.CreateAsync(mockPrincipal.Object, amphora, newRule);
            Assert.True(createRes.Succeeded);
            Assert.NotNull(createRes.Entity.Id);

            // now delete it
            var deleteRes = await sut.DeleteAsync(mockPrincipal.Object, amphora, createRes.Entity.Id);
            Assert.True(deleteRes.Succeeded);
        }

        [Fact]
        public async Task GiveUserAccess_SendsAnEmail()
        {
            var mockPrincipal = MockClaimsPrincipal();
            var context = GetContext();
            var ruleStore = new AmphoraAccessControlsEFStore(context, CreateMockLogger<AmphoraAccessControlsEFStore>());
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org);
            var thisUser = new ApplicationUserDataModel("thisUser", "thisUser", null, null);
            var contactEmail = $"{System.Guid.NewGuid().ToString()}@amphoradata.com";
            var userToGiveAccessTo = new ApplicationUserDataModel("otherUser", "otherUser", null,
                new ContactInformation(contactEmail, "Name"));
            var mockUserDataService = new Mock<IUserDataService>();
            mockUserDataService.Setup(_ => _.ReadAsync(mockPrincipal.Object, null))
                .ReturnsAsync(new Common.Models.EntityOperationResult<ApplicationUserDataModel>(thisUser, thisUser));
            var mockPermissionService = new Mock<IPermissionService>();

            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(
                    It.IsAny<IUser>(),
                    amphora,
                    It.IsInRange(AccessLevels.Read, AccessLevels.Administer, Range.Inclusive)))
                .ReturnsAsync(true);

            var mockEmailSender = new Mock<IEmailSender>();

            var sut = new AccessControlService(ruleStore,
                                               mockUserDataService.Object,
                                               mockPermissionService.Object,
                                               mockEmailSender.Object);

            var newRule = new UserAccessRule(Kind.Allow, 100, userToGiveAccessTo);
            var createRes = await sut.CreateAsync(mockPrincipal.Object, amphora, newRule);
            Assert.True(createRes.Succeeded);
            Assert.NotNull(createRes.Entity.Id);

            mockEmailSender.Verify(_ => _.SendEmailAsync(It.Is<GivenAccessToAmphoraEmail>(_ => _.Recipients[0].Email == contactEmail)));

            // now delete it
            var deleteRes = await sut.DeleteAsync(mockPrincipal.Object, amphora, createRes.Entity.Id);
            Assert.True(deleteRes.Succeeded);
        }
    }
}