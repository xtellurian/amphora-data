using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Stores.InMemory;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services.InMemory
{
    public class InMemoryAmphoraBlobTests : UnitTestBase
    {
        private RandomGenerator generator = new RandomGenerator();

        [Fact]
        public async Task CanAddAndGetAgain()
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org, setId: true);
            var generator = new RandomGenerator();
            var data = generator.GenerateBufferFromSeed(1024);
            var sut = new InMemoryAmphoraBlobStore(GetMockDateTimeProvider(), CreateMockLogger<InMemoryAmphoraBlobStore>());

            await sut.WriteBytesAsync(amphora, "path", data);
            var read = await sut.ReadBytesAsync(amphora, "path");

            ByteArrayCompare(data, read).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(64)]
        [InlineData(65)]
        [InlineData(99)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1001)]
        public async Task CanAddListGetAgain_Serially(int num)
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org, setId: true);
            var sut = new InMemoryAmphoraBlobStore(GetMockDateTimeProvider(), CreateMockLogger<InMemoryAmphoraBlobStore>());

            for (var n = 1; n <= num; n++)
            {
                await AddFileAndTest(amphora, sut, n, num + 1);
                (await sut.CountFilesAsync(amphora)).Should().Be(n);
            }

            var allFiles = await sut.GetFilesAsync(amphora, take: num + 1);
            allFiles.Should().HaveCount(num, "because we add 1 file per task");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        [InlineData(200)]
        public async Task CanAddListGetAgain_Concurrently(int num)
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org, setId: true);

            var sut = new InMemoryAmphoraBlobStore(GetMockDateTimeProvider(), CreateMockLogger<InMemoryAmphoraBlobStore>());

            var tasks = new List<Task>();
            for (var n = 1; n <= num; n++)
            {
                var order = n;
                tasks.Add(Task.Run(() => AddFileAndTest(amphora, sut, order, num + 1)));
            }

            tasks.Count.Should().Be(num);
            await Task.WhenAll(tasks);
            foreach (var t in tasks)
            {
                t.IsCompletedSuccessfully.Should().BeTrue();
            }

            var allFiles = await sut.GetFilesAsync(amphora, take: num + 1);
            allFiles.Should().HaveCount(num, "because we add 1 file per task");
        }

        private async Task AddFileAndTest(Common.Models.Amphorae.AmphoraModel amphora, InMemoryAmphoraBlobStore sut, int n, int maxN)
        {
            var name = $"path{n}";
            var data = generator.GenerateBufferFromSeed(1024);

            await sut.WriteBytesAsync(amphora, name, data);
            var read = await sut.ReadBytesAsync(amphora, name);
            ByteArrayCompare(data, read).Should().BeTrue($"because the {n}th file should match");

            var files = await sut.GetFilesAsync(amphora, name.Substring(0, 2), take: maxN);
            files.Any(_ => _.Name == name).Should().BeTrue($"because we added the {n}th file with name {name}, but there were {files.Count} files");
            files.Should().NotBeEmpty().And.Contain(f => f.Name == name);
        }
    }
}