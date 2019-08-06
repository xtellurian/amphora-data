using Xunit;
using Amphora.Api.Stores;
using Amphora.Api.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Amphora.Tests.Unit
{
    public class AmphoraStoreTests
    {

        [Fact]
        public async Task InMemoryCanStoreAndRetrieveAmphoraAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = new Amphora.Common.Models.Amphora()
            {
                Title = "Test Title",
                Description = "Test Description",
                Price = 42
            };

            await StoreAndRetrieveAmphoraAsync(sut, a);
        }

        [Fact]
        public async Task InMemoryEntityStoreListAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = new Amphora.Common.Models.Amphora()
            {
                Title = "Test Title",
                Description = "Test Description",
                Price = 42
            };

            await ListAmphoraAsync(sut, a);
        }

        [Fact]
        public async Task InMemoryEntityStoreReturnsNullAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            await GetMissingAmphoraAsync(sut);
        }

        private async Task GetMissingAmphoraAsync(IEntityStore<Amphora.Common.Models.Amphora> sut)
        {
            var missing = await sut.GetAsync("missing");
            Assert.Null(missing);
        }

        private async Task ListAmphoraAsync(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
        {
            var emptyList = await sut.ListAsync();
            Assert.Empty(emptyList);
            var setResult = await sut.SetAsync(a);
            var list = await sut.ListAsync();
            Assert.Single(list);
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private async Task StoreAndRetrieveAmphoraAsync(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
        {
            var setResult = await sut.SetAsync(a);
            Assert.NotNull(setResult);
            Assert.NotNull(setResult.Id);

            var id = a.Id;

            var getResult = await sut.GetAsync(id);
            Assert.Equal(id, getResult.Id);
            Assert.Equal(setResult, getResult);
        }
    }
}
