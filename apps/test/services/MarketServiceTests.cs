using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Tests.Helpers;
using AutoMapper;
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
        private InMemoryEntityStore<PermissionModel> permissionStore;
        private Mock<IUserManager> mockUserManager;
        private PermissionService permissionService;

        public MarketServiceTests(ILogger<PermissionService> permissionLogger, ILogger<AmphoraeService> amphoraLogger)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Startup));
            });
            var mapper = config.CreateMapper();
            this.amphoraStore = new InMemoryEntityStore<AmphoraModel>(Mapper);
            this.orgStore = new InMemoryEntityStore<OrganisationModel>(Mapper);
            this.permissionStore = new InMemoryEntityStore<PermissionModel>(Mapper);
            this.mockUserManager = new Mock<IUserManager>();
            this.orgStore = new InMemoryEntityStore<OrganisationModel>(mapper);
            this.permissionService = new PermissionService(permissionLogger, orgStore, permissionStore);
            this.amphoraLogger = amphoraLogger;
        }

        [Fact]
        public async Task NullsReturnEmptyList()
        {
            await AddToStore();
            var amphoraService = new AmphoraeService(amphoraStore, orgStore, permissionService, mockUserManager.Object, amphoraLogger);
            var service = new BasicSearchService(amphoraService);
            var sut = new MarketService(service) as IMarketService;

            var response = await sut.FindAsync(null);
            Assert.NotNull(response);
            Assert.Empty(response);
        }

        [Fact]
        public async Task LookupByGeo()
        {
            var amphoraService = new AmphoraeService(amphoraStore, orgStore, permissionService, mockUserManager.Object, amphoraLogger);
            var service = new BasicSearchService(amphoraService);
            var entity = await AddToStore();
            var sut = new MarketService(service) as IMarketService;

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