using System;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.EntityFramework;
using Amphora.Api.Stores.EFCore;
using Amphora.Identity.EntityFramework;
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
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var org = EntityLibrary.GetOrganisationModel();
                org = await orgStore.CreateAsync(org);
                var a = EntityLibrary.GetAmphoraModel(org);
                a = await amphoraStore.CreateAsync(a);

                var signal = EntityLibrary.GetV2Signal();
                Assert.NotNull(signal.Id);
                Assert.True(a.TryAddSignal(signal, out var message));
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

        [Fact]
        public void AmphoraContext_GetTypeName_IsAmphoraContext()
        {
            Assert.Equal(nameof(AmphoraContext), typeof(AmphoraContext).Name);
        }

        [Fact]
        public void IdentityContext_GetTypeName_IsIdentityContext()
        {
            Assert.Equal(nameof(IdentityContext), typeof(IdentityContext).Name);
        }
    }
}