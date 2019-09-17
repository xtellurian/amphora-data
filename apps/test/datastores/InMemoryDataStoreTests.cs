using System;
using System.Threading.Tasks;
using Amphora.Api.Stores;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit.Datastores
{
    public class InMemoryDataStoreTests
    {
        [Fact]
        public async Task ListNamesTest_Amphora()
        {
            var sut = new InMemoryBlobStore<AmphoraModel>();
            var entity = EntityLibrary.GetAmphora(Guid.NewGuid().ToString(), nameof(ListNamesTest_Amphora));
            entity.Id = Guid.NewGuid().ToString().AsQualifiedId<AmphoraModel>();
            var names = await sut.ListBlobsAsync(entity);
            Assert.Empty(names);

            var generator = new RandomBufferGenerator(1024);
            var data = generator.GenerateBufferFromSeed(1024);
            var name = Guid.NewGuid().ToString();

            await sut.WriteBytesAsync(entity, name, data);

            var getResult = await sut.ReadBytesAsync(entity, name);
            Assert.Equal(data, getResult);
        }
    }
}