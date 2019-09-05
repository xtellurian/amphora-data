using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Authorization;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
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

        public AmphoraAuthTests(ILogger<AmphoraAuthorizationHandler> logger)
        {
            this.logger = logger;
        }

        [Fact]
        public async Task DenyAllByDefault()
        {
            var principal = new TestPrincipal();
            var userManager = new Mock<IUserManager>();
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString() };

            userManager.Setup(_ => _.GetUserAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user as IApplicationUser));

            var amphoraStore = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = EntityLibrary.GetAmphora();
            a = await amphoraStore.CreateAsync(a);

            var authStore = new InMemoryEntityStore<ResourceAuthorization>();
            var handler = new AmphoraAuthorizationHandler(logger, authStore, userManager.Object);
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
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString() };

            userManager.Setup(_ => _.GetUserAsync(It.Is<ClaimsPrincipal>(p => p == principal))).Returns(Task.FromResult(user as IApplicationUser));

            var amphoraStore = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = EntityLibrary.GetAmphora();
            a = await amphoraStore.CreateAsync(a);

            var authStore = new InMemoryEntityStore<ResourceAuthorization>();
            var readPermission = new ResourceAuthorization()
            {
                ResourcePermission = ResourcePermissions.Read,
                TargetResourceId = a.Id,
                UserId = user.Id
            };

            await authStore.CreateAsync(readPermission);

            var handler = new AmphoraAuthorizationHandler(logger, authStore, userManager.Object);
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