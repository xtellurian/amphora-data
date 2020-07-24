using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class TermsOfUseServiceTests : UnitTestBase
    {
        [Fact]
        public async Task CanCreate_TermsOfUse_Success()
        {
            var context = GetContext();
            var touStore = new TermsOfUseEFStore(context, CreateMockLogger<TermsOfUseEFStore>());

            var fakeUserData = new ApplicationUserDataModel
            {
            };

            var mockPrincipal = MockClaimsPrincipal();
            var mockUserDataService = CreateMockUserDataService(new ConnectUser(fakeUserData, mockPrincipal.Object));
            var permission = CreateMockPermissionService();
            var sut = new TermsOfUseService(touStore, mockUserDataService.Object, permission, CreateMockLogger<TermsOfUseService>());

            var tou = new TermsOfUseModel("name of terms", "contents");
            var res = await sut.CreateAsync(mockPrincipal.Object, tou);

            Assert.True(res.Succeeded);
            Assert.NotNull(res.Entity);
            Assert.NotNull(res.Entity.Id);
            Assert.NotEmpty(context.TermsOfUse);
        }

        [Fact]
        public async Task CanDelete_TermsOfUse_AsTheCreator()
        {
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var touStore = new TermsOfUseEFStore(context, CreateMockLogger<TermsOfUseEFStore>());

            var creatorPrincipal = MockClaimsPrincipal().Object;
            var creatorOrg = await orgStore.CreateAsync(Helpers.EntityLibrary.GetOrganisationModel());
            var creator = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString(),
                OrganisationId = creatorOrg.Id,
                Organisation = creatorOrg
            };
            creatorOrg.Memberships.Add(new Common.Models.Organisations.Membership(creator.Id) { Role = Roles.Administrator });
            creatorOrg = await orgStore.UpdateAsync(creatorOrg);
            var attackerPrincipal = MockClaimsPrincipal().Object;
            var attackerOrg = await orgStore.CreateAsync(Helpers.EntityLibrary.GetOrganisationModel());
            var attacker = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString(),
                OrganisationId = attackerOrg.Id,
                Organisation = attackerOrg
            };
            attackerOrg.Memberships.Add(new Common.Models.Organisations.Membership(attacker.Id) { Role = Roles.Administrator });
            attackerOrg = await orgStore.UpdateAsync(attackerOrg);

            var mockUserDataService = MockUser(creatorPrincipal, creator);
            mockUserDataService = MockUser(attackerPrincipal, attacker);

            // need the real permission service for these tests to pass.
            var permission = GetPermissionService(context, mockUserDataService.Object);
            var sut = new TermsOfUseService(touStore, mockUserDataService.Object, permission, CreateMockLogger<TermsOfUseService>());

            var tou = new TermsOfUseModel("name of terms", "contents");
            var res = await sut.CreateAsync(creatorPrincipal, tou);
            Assert.True(res.Succeeded);

            // try to delete/ update as attacker
            var attackUpdate = await sut.UpdateAsync(attackerPrincipal, tou);
            Assert.False(attackUpdate.Succeeded);
            var attackDelete = await sut.DeleteAsync(attackerPrincipal, tou);
            Assert.False(attackDelete.Succeeded);

            // now try as creator
            var creatorUpdate = await sut.UpdateAsync(creatorPrincipal, tou);
            Assert.True(creatorUpdate.Succeeded);
            var creatorDelete = await sut.DeleteAsync(creatorPrincipal, tou);
            Assert.True(creatorDelete.Succeeded);
        }

        [Fact]
        public async Task RegularUser_ThrowsOnCreateGlobal()
        {
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var touStore = new TermsOfUseEFStore(context, CreateMockLogger<TermsOfUseEFStore>());
            var permissionSvc = Mock.Of<IPermissionService>();
            var creatorPrincipal = MockClaimsPrincipal().Object;
            var creator = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString()
            };
            var mockUserDataService = MockUser(creatorPrincipal, creator);

            var sut = new TermsOfUseService(touStore, mockUserDataService.Object, permissionSvc, CreateMockLogger<TermsOfUseService>());

            var tou = new TermsOfUseModel("name of terms", "contents");
            await Assert.ThrowsAsync<Common.Exceptions.PermissionDeniedException>(async () =>
            {
                var res = await sut.CreateGlobalAsync(creatorPrincipal, tou);
            });
        }

        [Fact]
        public async Task GlobalAdmin_CanCreateGlobal()
        {
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var touStore = new TermsOfUseEFStore(context, CreateMockLogger<TermsOfUseEFStore>());
            var permissionSvc = Mock.Of<IPermissionService>();
            var mockCreatorPrincipal = MockClaimsPrincipal();
            mockCreatorPrincipal.Setup(_ => _.Claims).Returns(new List<Claim>
            {
                new Claim(Common.Security.Claims.GlobalAdmin, true.ToString())
            });

            var adminPrincipal = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString()
            };
            var mockUserDataService = MockUser(mockCreatorPrincipal.Object, adminPrincipal);

            var sut = new TermsOfUseService(touStore, mockUserDataService.Object, permissionSvc, CreateMockLogger<TermsOfUseService>());

            var tou = new TermsOfUseModel("name of terms", "contents");
            var res = await sut.CreateGlobalAsync(mockCreatorPrincipal.Object, tou);
            Assert.True(res.Succeeded);
        }
    }
}