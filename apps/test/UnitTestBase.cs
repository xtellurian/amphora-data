using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Amphora.Api;
using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Wrappers;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Users;
using Amphora.Tests.Mocks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Amphora.Tests.Unit
{
    public abstract class UnitTestBase
    {
        // byte[] is implicitly convertible to ReadOnlySpan<byte>
        public static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        protected UnitTestBase()
        {
            var config = new MapperConfiguration(cfg =>
           {
               cfg.AddMaps(typeof(Startup));
           });
            this.Mapper = config.CreateMapper();
        }

        protected ILogger<T> CreateMockLogger<T>()
        {
            var logger = Mock.Of<ILogger<T>>();
            return logger;
        }

        protected IEventRoot CreateMockEventPublisher()
        {
            return Mock.Of<IEventRoot>();
        }

        protected Mock<ClaimsPrincipal> MockClaimsPrincipal()
        {
            return new Mock<ClaimsPrincipal>();
        }

        protected IPermissionService CreateMockPermissionService()
        {
            return Mock.Of<IPermissionService>();
        }

        protected IOptionsMonitor<T> MockOptions<T>(T o) where T : class
        {
            return Mock.Of<IOptionsMonitor<T>>(_ => _.CurrentValue == o);
        }

        protected Mock<IUserDataService> CreateMockUserDataService(ApplicationUserDataModel fakeUserData)
        {
            var md = new Mock<IUserDataService>();
            md
                .Setup(_ => _.ReadAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
                .ReturnsAsync(new EntityOperationResult<ApplicationUserDataModel>(fakeUserData, fakeUserData));

            return md;
        }

        private Mock<IUserDataService> mockUserDataService = new Mock<IUserDataService>();

        protected Mock<IUserDataService> MockUser(ClaimsPrincipal principal, ApplicationUserDataModel userData)
        {
            mockUserDataService
                .Setup(_ => _.ReadAsync(It.Is<ClaimsPrincipal>(_ => _ == principal), It.IsAny<string>()))
                .ReturnsAsync(new EntityOperationResult<ApplicationUserDataModel>(userData, userData));

            return mockUserDataService;
        }

        protected IPermissionService GetPermissionService(AmphoraContext context, IUserDataService userDataService)
        {
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            return new PermissionService(orgStore, amphoraStore, userDataService, CreateMockLogger<PermissionService>());
        }

        protected IDateTimeProvider GetMockDateTimeProvider(DateTimeOffset? now = null)
        {
            return now.HasValue ? new MockDateTimeProvider(now.Value) : new MockDateTimeProvider();
        }

        protected IMemoryCache CreateMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            return memoryCache;
        }

        protected ICache CreateCache()
        {
            return new InMemoryCache();
        }

        private Dictionary<string, DbContext> contexts = new Dictionary<string, DbContext>();
        protected AmphoraContext GetContext([CallerMemberName] string databaseName = "")
        {
            return this.GetContext<AmphoraContext>(databaseName);
        }

        protected T GetContext<T>([CallerMemberName] string databaseName = "") where T : DbContext
        {
            databaseName = $"{typeof(T)}|{databaseName}"; // prepend the type
            if (contexts.TryGetValue(databaseName, out var x))
            {
                return (T)x;
            }
            else
            {
                var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

                // Run the test against one instance of the context
                var context = Activator.CreateInstance(typeof(T), new object[] { options }) as T;
                contexts[databaseName] = context;
                return context;
            }
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        protected IMapper Mapper { get; }
    }
}