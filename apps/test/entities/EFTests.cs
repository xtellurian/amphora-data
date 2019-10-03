using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Stores.EFCore;
using Amphora.Tests.Helpers;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class EFTests : UnitTestBase
    {
        [Fact]
        public async Task AddSignalToAmphora()
        {
            using (var context = base.GetContext(nameof(AddSignalToAmphora)))
            {
                var testName = nameof(AddSignalToAmphora);
                var amphoraStore = new AmphoraeEFStore(context);
                var orgStore = new OrganisationsEFStore(context);
                var org = EntityLibrary.GetOrganisationModel(testName);
                org = await orgStore.CreateAsync(org);
                var a = EntityLibrary.GetAmphoraModel(org, testName);
                a = await amphoraStore.CreateAsync(a);

                var signal = EntityLibrary.GetSignalModel(testName);
                a.AddSignal(signal);

                a = await amphoraStore.UpdateAsync(a);

                Assert.NotNull(signal.Id);
                
                a = await amphoraStore.ReadAsync(a.Id, true);
                Assert.NotNull(a.Signals);
                Assert.Contains(a.Signals, m => string.Equals(m.SignalId, signal.Id));

                var s2 = context.Signals.FirstOrDefault(s => s.Id == signal.Id);
                Assert.NotNull(s2);
            }
        }
    }
}