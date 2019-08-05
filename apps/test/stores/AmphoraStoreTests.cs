using System;
using Newtonsoft.Json.Linq;
using Amphora.Schemas.Library;
using Xunit;
using Amphora.Common.Models;
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
            var sut = new InMemoryEntityStore<AmphoraModel>();
            var a = new AmphoraModel()
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
            var sut = new InMemoryEntityStore<AmphoraModel>();
            var a = new AmphoraModel()
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
            var sut = new InMemoryEntityStore<AmphoraModel>();
            GetMissingAmphora(sut);
        }

        private void GetMissingAmphora(IAmphoraEntityStore<AmphoraModel> sut)
        {
            var missing = sut.Get("missing");
            Assert.Null(missing);
        }

        private void ListAmphora(IAmphoraEntityStore<AmphoraModel> sut, AmphoraModel a)
        {
            var emptyList = sut.List();
            Assert.Equal(0, emptyList.Count());
            var setResult = sut.Set(a);
            var list = sut.List();
            Assert.Equal(1, list.Count());
            Assert.NotNull(list.FirstOrDefault());
            Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
        }

        private void StoreAndRetrieveAmphora(IAmphoraEntityStore<AmphoraModel> sut, AmphoraModel a)
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
