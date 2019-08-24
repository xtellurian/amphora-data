using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Services;
using Amphora.Common.Models;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class MarketServiceTests
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> store;

        public MarketServiceTests(IOrgScopedEntityStore<Amphora.Common.Models.Amphora> store)
        {
            this.store = store;
        }

        [Fact]
        public async Task NullsReturnEmptyList()
        {
            await AddToStore();
            var sut = new MarketService(store) as IMarketService;

            var response = await sut.FindAsync(null);
            Assert.NotNull(response);
            Assert.Empty(response);
        }

        [Fact]
        public async Task LookupByGeo()
        {
            var entity = await AddToStore();
            var sut = new MarketService(store) as IMarketService;
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

        private async Task<Amphora.Common.Models.Amphora> AddToStore()
        {
            var amphora = EntityLibrary.GetValidAmphora();
            return await store.CreateAsync(amphora);
        }
    }
}