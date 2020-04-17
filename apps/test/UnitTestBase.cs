using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Amphora.Api;
using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Api.Services.Wrappers;
using Amphora.Common.Contracts;
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

        protected IEventPublisher CreateMockEventPublisher()
        {
            return Mock.Of<IEventPublisher>();
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

        private Dictionary<string, AmphoraContext> contexts = new Dictionary<string, AmphoraContext>();
        protected AmphoraContext GetContext([CallerMemberName] string databaseName = "")
        {
            if (contexts.TryGetValue(databaseName, out var x))
            {
                return x;
            }
            else
            {
                var options = new DbContextOptionsBuilder<AmphoraContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

                // Run the test against one instance of the context
                var context = new AmphoraContext(options);
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