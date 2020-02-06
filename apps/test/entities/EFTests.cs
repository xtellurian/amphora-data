using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
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
            using (var context = GetContext(nameof(AddSignalToAmphora)))
            {
                var testName = nameof(AddSignalToAmphora);
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var org = EntityLibrary.GetOrganisationModel(testName);
                org = await orgStore.CreateAsync(org);
                var a = EntityLibrary.GetAmphoraModel(org, testName);
                a = await amphoraStore.CreateAsync(a);

                var signal = EntityLibrary.GetV2Signal();
                Assert.NotNull(signal.Id);
                a.V2Signals.Add(signal);
                a = await amphoraStore.UpdateAsync(a);

                Assert.NotNull(signal.Id);

                a = await amphoraStore.ReadAsync(a.Id);
                Assert.NotNull(a.V2Signals);
                Assert.Contains(a.V2Signals, m => string.Equals(m.Id, signal.Id));
            }
        }

        [Fact]
        public void EFCore_ProducesDGML()
        {
            var context = GetContext();
            var path = Environment.GetEnvironmentVariable("DGML_PATH");
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory() + "/Entities.dgml";
            }

            System.IO.File.WriteAllText(path, context.AsDgml(), System.Text.Encoding.UTF8);
        }
    }
}