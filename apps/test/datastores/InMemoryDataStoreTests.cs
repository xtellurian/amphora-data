using System;
using System.Threading.Tasks;
using Amphora.Api.Stores;
using Amphora.Common.Extensions;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit.Datastores
{
    public class InMemoryDataStoreTests
    {
        [Fact]
        public async Task ListNamesTest_Amphora()
        {
            var sut = new InMemoryDataStore<Amphora.Common.Models.AmphoraModel, byte[]>();
            var entity = EntityLibrary.GetAmphora(Guid.NewGuid().ToString());
            entity.Id = Guid.NewGuid().ToString().AsQualifiedId<Amphora.Common.Models.AmphoraModel>();
            var names = await sut.ListNamesAsync(entity);
            Assert.Empty(names);

            var generator = new RandomBufferGenerator(1024);
            var data = generator.GenerateBufferFromSeed(1024);
            var name = Guid.NewGuid().ToString();

            var setResult = await sut.SetDataAsync(entity, data, name);
            Assert.Equal(data, setResult);

            var getResult = await sut.GetDataAsync(entity, name);
            Assert.Equal(data, getResult);
        }
    }
}