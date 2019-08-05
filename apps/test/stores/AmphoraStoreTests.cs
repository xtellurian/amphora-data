using Xunit;
using Amphora.Api.Stores;
using Amphora.Api.Contracts;
using System.Linq;

namespace Amphora.Tests.Unit
{
    public class AmphoraStoreTests
    {

        [Fact]
        public void InMemoryCanStoreAndRetrieveAmphora()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = new Amphora.Common.Models.Amphora()
            {
                Title = "Test Title",
                Description = "Test Description",
                SchemaId = "not a real schema I reckon",
                Price = 42
            };

            StoreAndRetrieveAmphora(sut, a);
        }

        [Fact]
        public void InMemoryEntityStoreList()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            var a = new Amphora.Common.Models.Amphora()
            {
                Title = "Test Title",
                Description = "Test Description",
                SchemaId = "not a real schema I reckon",
                Price = 42
            };

            ListAmphora(sut, a);
        }

        [Fact]
        public void InMemoryEntityStoreReturnsNull()
        {
            var sut = new InMemoryEntityStore<Amphora.Common.Models.Amphora>();
            GetMissingAmphora(sut);
        }

        private void GetMissingAmphora(IEntityStore<Amphora.Common.Models.Amphora> sut)
        {
            var missing = sut.Get("missing");
            Assert.Null(missing);
        }

        private void ListAmphora(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
        {
            var emptyList = sut.List();
            Assert.Empty(emptyList);
            var setResult = sut.Set(a);
            var list = sut.List();
            Assert.Single(list);
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private void StoreAndRetrieveAmphora(IEntityStore<Amphora.Common.Models.Amphora> sut, Amphora.Common.Models.Amphora a)
        {
            var setResult = sut.Set(a);
            Assert.NotNull(setResult);
            Assert.NotNull(setResult.Id);

            var id = a.Id;

            var getResult = sut.Get(id);
            Assert.Equal(id, getResult.Id);
            Assert.Equal(setResult, getResult);
        }
    }
}
