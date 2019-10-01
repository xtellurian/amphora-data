using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Auth;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Authorization
{
    public class AmphoraAuthTests: UnitTestBase
    {
        private readonly ILogger<AmphoraAuthorizationHandler> logger;
        private readonly ILogger<PermissionService> permissionServiceLogger;

        public AmphoraAuthTests(ILogger<AmphoraAuthorizationHandler> logger,
                                ILogger<PermissionService> permissionServiceLogger)
        {
            this.logger = logger;
            this.permissionServiceLogger = permissionServiceLogger;
        }

        [Fact]
        public async Task DenyAllByDefault()
        {
            var principal = new TestPrincipal();
            var userService = new Mock<IUserService>();
            var org = EntityLibrary.GetOrganisationModel(nameof(DenyAllByDefault));
            using (var context = GetContext(nameof(DenyAllByDefault)))
            {
                var orgStore = new OrganisationsEFStore(context);
                org = await orgStore.CreateAsync(org);
                var user = new ApplicationUser { Id = Guid.NewGuid().ToString() };

                userService.Setup(_ => _.ReadUserModelAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user));

                var amphoraStore = new AmphoraeEFStore(context);
                var a = EntityLibrary.GetAmphoraModel(org, nameof(DenyAllByDefault));
                a = await amphoraStore.CreateAsync(a);

                var permissionService = new PermissionService(permissionServiceLogger, orgStore, amphoraStore);
                var handler = new AmphoraAuthorizationHandler(logger, permissionService, userService.Object);

                var readReq = new List<IAuthorizationRequirement> { Operations.Read };
                var createReq = new List<IAuthorizationRequirement> { Operations.Create };
                var updateReq = new List<IAuthorizationRequirement> { Operations.Update };
                var deleteReq = new List<IAuthorizationRequirement> { Operations.Delete };

                var authContext = new AuthorizationHandlerContext(readReq, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
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
            var userService = new Mock<IUserService>();
            var org = EntityLibrary.GetOrganisationModel(nameof(OrgMember_ReadAccess_Amphora));
            using (var context = GetContext(nameof(OrgMember_ReadAccess_Amphora)))
            {
                var orgStore = new OrganisationsEFStore(context);
                org = await orgStore.CreateAsync(org);
                var extendedOrg = await orgStore.ReadAsync(org.Id);
                var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), OrganisationId = org.Id };
                extendedOrg.AddOrUpdateMembership(user);
                await orgStore.UpdateAsync(extendedOrg);

                userService.Setup(_ => _.ReadUserModelAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user));

                var amphoraStore = new AmphoraeEFStore(context);
                var a = EntityLibrary.GetAmphoraModel(org, nameof(OrgMember_ReadAccess_Amphora));
                a = await amphoraStore.CreateAsync(a);

                var permissionService = new PermissionService(permissionServiceLogger, orgStore, amphoraStore);

                var handler = new AmphoraAuthorizationHandler(logger, permissionService, userService.Object);
                var requirements = new List<IAuthorizationRequirement> { Operations.Read };
                var authContext = new AuthorizationHandlerContext(requirements, principal, a);
                await handler.HandleAsync(authContext);
                Assert.True(authContext.HasSucceeded);

                requirements.Add(Operations.Update);
                authContext = new AuthorizationHandlerContext(requirements, principal, a);
                await handler.HandleAsync(authContext);
                Assert.False(authContext.HasSucceeded);
            }
        }
    }
}