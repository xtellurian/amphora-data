using System.Threading.Tasks;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Stores.EFCore;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class TextAnalysisTests : UnitTestBase
    {
        [Fact]
        public async Task WordCountIsValid()
        {
            var context = GetContext();
            var store = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());

            var sut = new AmphoraeTextAnalysisService(store);

            // create some amphora
            var org = EntityLibrary.GetOrganisationModel();
            await store.CreateAsync(EntityLibrary.GetAmphoraModel(org));
            await store.CreateAsync(EntityLibrary.GetAmphoraModel(org));
            await store.CreateAsync(EntityLibrary.GetAmphoraModel(org));
            await store.CreateAsync(EntityLibrary.GetAmphoraModel(org));
            await store.CreateAsync(EntityLibrary.GetAmphoraModel(org));

            var freqs = sut.WordFrequencies();

            Assert.NotNull(freqs);
            Assert.NotEmpty(freqs);

            var result = sut.ToWordSizeList(freqs);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            foreach (var r in result)
            {
                Assert.Equal(2, r.Count); // check every list is 2 in length [word, count]
            }
        }
    }
}