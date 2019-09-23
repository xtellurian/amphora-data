using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Users;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class MarketServiceTests : UnitTestBase
    {
        private readonly ILogger<AmphoraeService> amphoraLogger;
        private InMemoryEntityStore<AmphoraModel> amphoraStore;
        private InMemoryEntityStore<OrganisationModel> orgStore;
        private Mock<IUserManager> mockUserManager;
        private Mock<IUserService> mockUserService;
        private PermissionService permissionService;

        public MarketServiceTests(ILogger<PermissionService> permissionLogger, ILogger<AmphoraeService> amphoraLogger)
        {
            this.amphoraStore = new InMemoryEntityStore<AmphoraModel>(Mapper);
            this.orgStore = new InMemoryEntityStore<OrganisationModel>(Mapper);
            this.mockUserManager = new Mock<IUserManager>();
            mockUserManager.Setup(o => o.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new TestApplicationUser());
            this.mockUserService = new Mock<IUserService>();
            this.mockUserService.Setup(o => o.UserManager).Returns(mockUserManager.Object);
            this.orgStore = new InMemoryEntityStore<OrganisationModel>(Mapper);
            this.permissionService = new PermissionService(permissionLogger, orgStore, amphoraStore);
            this.amphoraLogger = amphoraLogger;
        }

        [Fact]
        public async Task LookupByGeo()
        {
            var amphoraService = new AmphoraeService(amphoraStore,
                                                     orgStore,
                                                     permissionService,
                                                     mockUserManager.Object,
                                                     amphoraLogger);
            var service = new BasicSearchService(amphoraService);
            var entity = await AddToStore();
            var sut = new MarketService(service, amphoraService, Mapper, mockUserService.Object) as IMarketService;

            var response = await sut.FindAsync("");
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains(response, e => e.Id == entity.Id);
        }

        private async Task<AmphoraModel> AddToStore()
        {
            var amphora = EntityLibrary.GetAmphora("1234", nameof(MarketServiceTests)); // dumy org id
            return await amphoraStore.CreateAsync(amphora);
        }
    }
}