using Xunit;
using Amphora.Api.Stores;
using Amphora.Api.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Newtonsoft.Json;

namespace Amphora.Tests.Unit
{
    public class AmphoraEntityTests: UnitTestBase
    {

        [Fact]
        public async Task InMemoryCanStoreAndRetrieveAmphoraAsync()
        {
            var sut = new InMemoryEntityStore<AmphoraModel>(Mapper);
            var a = new AmphoraExtendedModel()
            {
                Name = "Test Name",
                Description = "Test Description",
                Price = 42
            };

            await StoreAndRetrieveAmphoraAsync(sut, a);
        }

        [Fact]
        public void CanSerialiseAndDeserialiseAmphora()
        {
            var a = EntityLibrary.GetAmphora("123", nameof(CanSerialiseAndDeserialiseAmphora));
            var a_serialised = JsonConvert.SerializeObject(a);
            var b = JsonConvert.DeserializeObject<AmphoraExtendedModel>(a_serialised);
            Assert.NotNull(b);
            Assert.Equal(a.OrganisationId, b.OrganisationId);
        }

        [Fact]
        public async Task InMemoryEntityStoreListAsync()
        {
            var sut = new InMemoryEntityStore<AmphoraModel>(Mapper);
            var a = new AmphoraExtendedModel()
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
            var sut = new InMemoryEntityStore<AmphoraModel>(Mapper);
            await GetMissingAmphoraAsync(sut);
        }

        private async Task GetMissingAmphoraAsync(IEntityStore<AmphoraModel> sut)
        {
            var missing = await sut.ReadAsync("missing");
            Assert.Null(missing);
        }

        private async Task ListAmphoraAsync(IEntityStore<AmphoraModel> sut, AmphoraModel a)
        {
            var emptyList = await sut.TopAsync();
            Assert.Empty(emptyList);
            var setResult = await sut.CreateAsync(a);
            var list = await sut.TopAsync();
            Assert.Single(list);
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private async Task StoreAndRetrieveAmphoraAsync(IEntityStore<AmphoraModel> sut, AmphoraModel a)
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
