using System;
using Newtonsoft.Json.Linq;
using Amphora.Schemas.Library;
using Xunit;
using Amphora.Common.Models;
using Amphora.Api.Stores;
using Amphora.Api.Contracts;

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
                Title= "Test Title",
                Description = "Test Description",
                SchemaId = "not a real schema I reckon",
                Price = 42
            };

            StoreAndRetrieveAmphora(sut, a);
            
        }

        private void StoreAndRetrieveAmphora(IAmphoraEntityStore<AmphoraModel> sut, AmphoraModel a)
        {
            var setResult = sut.Set(a);
            Assert.NotNull(setResult);
            Assert.NotNull(setResult.Id);

        }
    }
}
