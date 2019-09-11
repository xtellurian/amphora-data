using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Services.Auth;
using Amphora.Api.Stores;
using Amphora.Common.Models;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Authorization
{
    public class AmphoraAuthTests
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
            var userManager = new Mock<IUserManager>();
            var org = EntityLibrary.GetOrganisation();
            var orgStore = new InMemoryEntityStore<OrganisationModel>();
            org = await orgStore.CreateAsync(org);
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), OrganisationId = org.OrganisationId };

            userManager.Setup(_ => _.GetUserAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user as IApplicationUser));

            var amphoraStore = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            var a = EntityLibrary.GetAmphora(org.OrganisationId);
            a = await amphoraStore.CreateAsync(a);

            var store = new InMemoryEntityStore<PermissionCollection>();
            var permissionService = new PermissionService(permissionServiceLogger, store);
            var handler = new AmphoraAuthorizationHandler(logger, permissionService, userManager.Object);

            var readReq = new List<IAuthorizationRequirement> { Operations.Read };
            var createReq = new List<IAuthorizationRequirement> { Operations.Create };
            var updateReq = new List<IAuthorizationRequirement> { Operations.Update };
            var deleteReq = new List<IAuthorizationRequirement> { Operations.Delete };

            var context = new AuthorizationHandlerContext(readReq, principal, a);
            await handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
            context = new AuthorizationHandlerContext(createReq, principal, a);
            await handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
            context = new AuthorizationHandlerContext(updateReq, principal, a);
            await handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
            context = new AuthorizationHandlerContext(deleteReq, principal, a);
            await handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task AllowReadOnAmphora()
        {
            var principal = new TestPrincipal();
            var userManager = new Mock<IUserManager>();
            var org = EntityLibrary.GetOrganisation();
            var orgStore = new InMemoryEntityStore<OrganisationModel>();
            org = await orgStore.CreateAsync(org);
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), OrganisationId = org.OrganisationId };

            userManager.Setup(_ => _.GetUserAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user as IApplicationUser));

            var amphoraStore = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            var a = EntityLibrary.GetAmphora(org.OrganisationId);
            a = await amphoraStore.CreateAsync(a);

            var store = new InMemoryEntityStore<PermissionCollection>();
            var permissionService = new PermissionService(permissionServiceLogger, store);

            var collection = new PermissionCollection(a.OrganisationId);
            var readPermission = new ResourceAuthorization()
            {
                ResourcePermission = ResourcePermissions.Read,
                TargetResourceId = a.Id,
                UserId = user.Id
            };
            collection.ResourceAuthorizations.Add(readPermission);

            await store.CreateAsync(collection);

            var handler = new AmphoraAuthorizationHandler(logger, permissionService, userManager.Object);
            var requirements = new List<IAuthorizationRequirement> { Operations.Read };
            var context = new AuthorizationHandlerContext(requirements, principal, a);
            await handler.HandleAsync(context);
            Assert.True(context.HasSucceeded);

            requirements.Add(Operations.Update);
            context = new AuthorizationHandlerContext(requirements, principal, a);
            await handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
        }
    }
}