using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Stores.EFCore;
using Amphora.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
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
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var org = EntityLibrary.GetOrganisationModel(testName);
                org = await orgStore.CreateAsync(org);
                var a = EntityLibrary.GetAmphoraModel(org, testName);
                a = await amphoraStore.CreateAsync(a);

                var signal = EntityLibrary.GetSignalModel(testName);
                a.AddSignal(signal);

                a = await amphoraStore.UpdateAsync(a);

                Assert.NotNull(signal.Id);

                a = await amphoraStore.ReadAsync(a.Id);
                Assert.NotNull(a.Signals);
                Assert.Contains(a.Signals, m => string.Equals(m.SignalId, signal.Id));

                var s2 = context.Signals.FirstOrDefault(s => s.Id == signal.Id);
                Assert.NotNull(s2);
            }
        }

        [Fact]
        public void EFCore_ProducesDGML()
        {
            var context = base.GetContext();
            var path = Environment.GetEnvironmentVariable("DGML_PATH");
            if(string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory() + "/Entities.dgml";
            }
            System.IO.File.WriteAllText(path, context.AsDgml(), System.Text.Encoding.UTF8);

        }
    }
}