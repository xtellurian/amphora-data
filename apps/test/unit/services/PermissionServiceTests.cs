using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Api.Services.Auth;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class PermissionServiceTests : UnitTestBase
    {
        [Fact]
        public async Task DenyAllButReadByDefault()
        {
            var principal = new TestPrincipal();
            var userDataService = new Mock<IUserDataService>();
            var org = EntityLibrary.GetOrganisationModel(nameof(DenyAllButReadByDefault));
            using (var context = GetContext(nameof(DenyAllButReadByDefault)))
            {
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                org = await orgStore.CreateAsync(org);
                var userData = new ApplicationUserDataModel { Id = Guid.NewGuid().ToString() };
                var userEntityRes = new EntityOperationResult<ApplicationUserDataModel>(userData, userData);
                userDataService.Setup(_ => _.ReadAsync(It.Is<ClaimsPrincipal>(p => p == principal), It.IsAny<string>())).Returns(Task.FromResult(userEntityRes));

                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var a = EntityLibrary.GetAmphoraModel(org, false); // ensure amphora is not public (auto deny access)
                a = await amphoraStore.CreateAsync(a);

                var permissionService = new PermissionService(orgStore, amphoraStore, userDataService.Object, CreateMockLogger<PermissionService>());
                var handler = new AmphoraAuthorizationHandler(CreateMockLogger<AmphoraAuthorizationHandler>(), permissionService, userDataService.Object);

                var readReq = new List<IAuthorizationRequirement> { new AuthorizationRequirement { MinimumLevel = AccessLevels.Read } };
                var updateReq = new List<IAuthorizationRequirement> { new AuthorizationRequirement { MinimumLevel = AccessLevels.Update } };
                var deleteReq = new List<IAuthorizationRequirement> { new AuthorizationRequirement { MinimumLevel = AccessLevels.Administer } };
                var createReq = new List<IAuthorizationRequirement> { new AuthorizationRequirement { MinimumLevel = AccessLevels.Administer } };

                var authContext = new AuthorizationHandlerContext(readReq, principal, a);
                await handler.HandleAsync(authContext);
                Assert.True(authContext.HasSucceeded);

                authContext = new AuthorizationHandlerContext(createReq, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
                authContext = new AuthorizationHandlerContext(updateReq, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
                authContext = new AuthorizationHandlerContext(deleteReq, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
            }
        }

        [Fact]
        public async Task OrgMember_ReadAccess_Amphora()
        {
            var principal = new TestPrincipal();
            var org = EntityLibrary.GetOrganisationModel(nameof(OrgMember_ReadAccess_Amphora));
            var userDataService = new Mock<IUserDataService>();
            using (var context = GetContext(nameof(OrgMember_ReadAccess_Amphora)))
            {
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                org = await orgStore.CreateAsync(org);
                var extendedOrg = await orgStore.ReadAsync(org.Id);
                var user = new ApplicationUserDataModel { Id = Guid.NewGuid().ToString(), OrganisationId = org.Id };
                extendedOrg.AddOrUpdateMembership(user);
                await orgStore.UpdateAsync(extendedOrg);

                var userData = new ApplicationUserDataModel { Id = Guid.NewGuid().ToString() };
                var userEntityRes = new EntityOperationResult<ApplicationUserDataModel>(userData, userData);
                userDataService.Setup(_ => _.ReadAsync(It.Is<ClaimsPrincipal>(p => p == principal), It.IsAny<string>())).Returns(Task.FromResult(userEntityRes));

                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var a = EntityLibrary.GetAmphoraModel(org);
                a = await amphoraStore.CreateAsync(a);

                var permissionService = new PermissionService(orgStore, amphoraStore, userDataService.Object, CreateMockLogger<PermissionService>());

                var handler = new AmphoraAuthorizationHandler(CreateMockLogger<AmphoraAuthorizationHandler>(), permissionService, userDataService.Object);
                var requirements = new List<IAuthorizationRequirement> { new AuthorizationRequirement { MinimumLevel = AccessLevels.Read } };
                var authContext = new AuthorizationHandlerContext(requirements, principal, a);
                await handler.HandleAsync(authContext);
                Assert.True(authContext.HasSucceeded);

                requirements.Add(new AuthorizationRequirement { MinimumLevel = AccessLevels.Update });
                authContext = new AuthorizationHandlerContext(requirements, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
            }
        }

        [Fact]
        public async Task UsersInSameOrg_CanReadContentsOfPurchased()
        {
            using (var context = GetContext())
            {
                // setup (lots of it)
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var purchasingOrg = EntityLibrary.GetOrganisationModel();
                purchasingOrg = await orgStore.CreateAsync(purchasingOrg);

                var purchasorPrincipal = new TestPrincipal();
                var purchasorUserData = new ApplicationUserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    OrganisationId = purchasingOrg.Id,
                    Organisation = purchasingOrg
                };
                var accessorPrincipal = new TestPrincipal();
                var accessorUserData = new ApplicationUserDataModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    OrganisationId = purchasingOrg.Id,
                    Organisation = purchasingOrg
                };

                var sellingOrg = EntityLibrary.GetOrganisationModel();
                sellingOrg = await orgStore.CreateAsync(sellingOrg);
                var sellerPrincipal = new TestPrincipal();
                var sellerUserData = new ApplicationUserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    OrganisationId = sellingOrg.Id,
                    Organisation = sellingOrg
                };

                var userDataService = CreateMockUserDataService(
                        new ConnectUser(purchasorUserData, purchasorPrincipal),
                        new ConnectUser(accessorUserData, accessorPrincipal),
                        new ConnectUser(sellerUserData, sellerPrincipal));

                purchasingOrg.AddOrUpdateMembership(purchasorUserData);
                purchasingOrg.AddOrUpdateMembership(accessorUserData);
                await orgStore.UpdateAsync(purchasingOrg);
                sellingOrg.AddOrUpdateMembership(sellerUserData);
                await orgStore.UpdateAsync(sellingOrg);

                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var amphora = EntityLibrary.GetAmphoraModel(sellingOrg);
                amphora = await amphoraStore.CreateAsync(amphora);

                // create the purchase entity + store
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var purchase = new PurchaseModel(purchasorUserData, amphora, DateTimeOffset.UtcNow);
                purchase = await purchaseStore.CreateAsync(purchase);

                // create the SUT
                var sut = new PermissionService(orgStore, amphoraStore, userDataService.Object, CreateMockLogger<PermissionService>());

                // create the purchase model
                var purchasorCanRead = await sut.IsAuthorizedAsync(purchasorPrincipal, amphora, AccessLevels.ReadContents);
                purchasorCanRead.Should().BeTrue("because purchasor can read the data");
                var accessorCanRead = await sut.IsAuthorizedAsync(accessorPrincipal, amphora, AccessLevels.ReadContents);
                accessorCanRead.Should().BeTrue("because accessor can also read the data");
            }
        }
    }
}