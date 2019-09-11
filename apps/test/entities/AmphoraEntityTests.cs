using Xunit;
using Amphora.Api.Stores;
using Amphora.Api.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Amphora.Tests.Unit
{
    public class AmphoraEntityTests
    {

        [Fact]
        public async Task InMemoryCanStoreAndRetrieveAmphoraAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            var a = new Amphora.Common.Models.AmphoraModel()
            {
                Name = "Test Name",
                Description = "Test Description",
                Price = 42
            };

            await StoreAndRetrieveAmphoraAsync(sut, a);
        }

        [Fact]
        public async Task InMemoryEntityStoreListAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            var a = new Amphora.Common.Models.AmphoraModel()
            {
                Name = "Test Name",
                Description = "Test Description",
                Price = 42
            };

            await ListAmphoraAsync(sut, a);
        }

        [Fact]
        public async Task InMemoryEntityStoreReturnsNullAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>();
            await GetMissingAmphoraAsync(sut);
        }

        private async Task GetMissingAmphoraAsync(IEntityStore<Amphora.Common.Models.AmphoraModel> sut)
        {
            var missing = await sut.ReadAsync("missing");
            Assert.Null(missing);
        }

        private async Task ListAmphoraAsync(IEntityStore<Amphora.Common.Models.AmphoraModel> sut, Amphora.Common.Models.AmphoraModel a)
        {
            var emptyList = await sut.ListAsync();
            Assert.Empty(emptyList);
            var setResult = await sut.CreateAsync(a);
            var list = await sut.ListAsync();
            Assert.Single(list);
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private async Task StoreAndRetrieveAmphoraAsync(IEntityStore<Amphora.Common.Models.AmphoraModel> sut, Amphora.Common.Models.AmphoraModel a)
        {
            var setResult = await sut.CreateAsync(a);
            Assert.NotNull(setResult);
            Assert.NotNull(setResult.Id);

            var id = a.Id;

            var getResult = await sut.ReadAsync(id);
            Assert.Equal(id, getResult.Id);
            Assert.Equal(setResult, getResult);
        }
    }
}
