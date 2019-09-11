using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class MarketServiceTests
    {
        private readonly ILogger<AmphoraeService> amphoraLogger;
        private InMemoryEntityStore<Common.Models.AmphoraModel> amphoraStore;
        private InMemoryEntityStore<OrganisationModel> orgStore;
        private InMemoryEntityStore<PermissionModel> permissionStore;
        private Mock<IUserManager> mockUserManager;
        private PermissionService permissionService;

        public MarketServiceTests(ILogger<PermissionService> permissionLogger, ILogger<AmphoraeService> amphoraLogger)
        {
            this.amphoraStore = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            this.orgStore = new InMemoryEntityStore<OrganisationModel>();
            this.permissionStore = new InMemoryEntityStore<Amphora.Common.Models.PermissionModel>();
            this.mockUserManager = new Mock<IUserManager>();
            this.permissionService = new PermissionService(permissionLogger, permissionStore);
            this.amphoraLogger = amphoraLogger;
        }

        [Fact]
        public async Task NullsReturnEmptyList()
        {
            await AddToStore();
            var service = new AmphoraeService(amphoraStore, orgStore, permissionService, mockUserManager.Object,  amphoraLogger );
            var sut = new MarketService(service) as IMarketService;

            var response = await sut.FindAsync(null);
            Assert.NotNull(response);
            Assert.Empty(response);
        }

        [Fact]
        public async Task LookupByGeo()
        {
            var service = new AmphoraeService(amphoraStore, orgStore, permissionService, mockUserManager.Object,  amphoraLogger );
            var entity = await AddToStore();
            var sut = new MarketService(service) as IMarketService;
            var searchParams = new SearchParams
            {
                IsGeoSearch = true,
                GeoHashStartsWith = entity.GeoHash.Substring(0,4)
            };
            var response = await sut.FindAsync(searchParams);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains(response, e => e.Id == entity.Id);
        }

        private async Task<Amphora.Common.Models.AmphoraModel> AddToStore()
        {
            var amphora = EntityLibrary.GetAmphora("1234"); // dumy org id
            return await amphoraStore.CreateAsync(amphora);
        }
    }
}