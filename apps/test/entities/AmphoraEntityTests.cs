using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Unit.Entities
{
    public class AmphoraEntityTests : UnitTestBase
    {
        [Fact]
        public async Task InMemoryCanStoreAndRetrieveAmphoraAsync()
        {
            using (var context = GetContext())
            {
                var sut = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());

                var a = new AmphoraModel("Test Name", "Test Description", 42, null, null, null);

                var setResult = await sut.CreateAsync(a);
                Assert.NotNull(setResult);
                Assert.NotNull(setResult.Id);

                var id = a.Id;

                var getResult = await sut.ReadAsync(id);
                Assert.Equal(id, getResult.Id);
                Assert.Equal(setResult, getResult);
            }
        }

        [Fact]
        public void CanSerialiseAndDeserialiseAmphora()
        {
            var a = EntityLibrary.GetAmphoraDto("123", nameof(CanSerialiseAndDeserialiseAmphora));
            var a_serialised = JsonConvert.SerializeObject(a);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(a_serialised);
            Assert.NotNull(b);
            Assert.Equal(a.OrganisationId, b.OrganisationId);
        }

        [Fact]
        public async Task InMemoryEntityStoreListAsync()
        {
            using (var context = GetContext())
            {
                var sut = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var a = new AmphoraModel("Test name", "Test Description", 44, null, null, null);

                var emptyList = await sut.TopAsync();
                Assert.Empty(emptyList);
                var setResult = await sut.CreateAsync(a);
                var list = await sut.TopAsync();
                Assert.Single(list);
                Assert.NotNull(list.FirstOrDefault());
                Assert.Equal(setResult.Id, list.FirstOrDefault().Id);
            }
        }

        [Fact]
        public async Task InMemoryEntityStoreReturnsNullAsync()
        {
            using (var context = GetContext())
            {
                var sut = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var missing = await sut.ReadAsync("missing_amphora");
                Assert.Null(missing);
            }
        }

        [Fact]
        public async Task DefaultNumberOfSignals_10()
        {
            using (var context = GetContext())
            {
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());

                var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
                var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(org));

                for (var i = 0; i < 10; i++)
                {
                    // add 10 successfully
                    Assert.True(amphora.TryAddSignal(EntityLibrary.GetSignalV2(), out var m));
                }

                // check it's 10
                Assert.Equal(10, amphora.V2Signals.Count);

                // then add the last one, should fail
                string message;
                Assert.False(amphora.TryAddSignal(EntityLibrary.GetSignalV2(), out message));
                Assert.False(string.IsNullOrEmpty(message));
            }
        }

        [Fact]
        public async Task CustomNumberOfSignals()
        {
            using (var context = GetContext())
            {
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());

                var customLimit = 21;
                var org = EntityLibrary.GetOrganisationModel();
                org.Configuration.MaximumSignals = customLimit;

                org = await orgStore.CreateAsync(org);
                var amphora = await amphoraStore.CreateAsync(EntityLibrary.GetAmphoraModel(org));

                for (var i = 0; i < customLimit; i++)
                {
                    // add 10 successfully
                    Assert.True(amphora.TryAddSignal(EntityLibrary.GetSignalV2(), out var m));
                }

                // check it's the custom limit
                Assert.Equal(customLimit, amphora.V2Signals.Count);

                // then add the last one, should fail
                string message;
                Assert.False(amphora.TryAddSignal(EntityLibrary.GetSignalV2(), out message));
                Assert.False(string.IsNullOrEmpty(message));
            }
        }
    }
}
