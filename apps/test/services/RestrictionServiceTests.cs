using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Auth;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class RestrictionServiceTests : UnitTestBase
    {
        [Fact]
        public async Task CreateRestriction_OnOrg_HappyPath()
        {
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var restrictionsStore = new RestrictionsEFStore(context, CreateMockLogger<RestrictionsEFStore>());

            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var otherOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var user = new ApplicationUser
            {
                OrganisationId = org.Id
            };
            // make sure the user is a admin
            org.AddOrUpdateMembership(user, Roles.Administrator);
            org = await orgStore.UpdateAsync(org);

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(_ => _.ReadUserModelAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());

            IRestrictionService sut = new RestrictionService(restrictionsStore,
                                                             mockUserService.Object,
                                                             permissionService,
                                                             CreateMockLogger<RestrictionService>());

            // Act
            var restriction = new RestrictionModel(org, otherOrg, RestrictionKind.Deny);
            var result = await sut.CreateAsync(null, restriction);

            Assert.True(result.Succeeded);
            Assert.Equal(200, result.Code);
            Assert.NotNull(result.Entity?.Id);
            Assert.Equal(user.OrganisationId, result.Entity.SourceOrganisationId);
        }
    }
}