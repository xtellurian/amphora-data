using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Stores;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class TemporaEntityTests
    {
        [Fact]
        public async Task InMemoryCanStoreAndRetrieveAmphoraAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = Helpers.EntityLibrary.GetValidAmphora();

            await StoreAndRetrieveAsync(sut, a);
        }

        [Fact]
        public async Task InMemoryEntityStoreListAsync()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = Helpers.EntityLibrary.GetValidAmphora();

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
            var missing = await sut.ReadAsync("missing");
            Assert.Null(missing);
        }

        private async Task ListAmphoraAsync(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
        {
            var emptyList = await sut.ListAsync();
            Assert.Empty(emptyList);
            var setResult = await sut.CreateAsync(a);
            var list = await sut.ListAsync();
            Assert.Single(list);
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private async Task StoreAndRetrieveAsync(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
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