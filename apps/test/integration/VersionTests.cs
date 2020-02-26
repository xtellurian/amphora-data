using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    [Trait("Category", "Versioning")]
    public class VersionTests : IntegrationTestBase
    {
        public VersionTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanGetVersion()
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/version");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            await AssertHttpSuccess(response);
            var version = ApiVersionIdentifier.FromSemver(content);
            Assert.NotNull(version.ToSemver());
        }

        [Fact]
        public async Task WhenEnvironmentVariable_IsIncrementingVersion()
        {
            var reason = System.Environment.GetEnvironmentVariable("BUILD_REASON");
            if (reason != null && reason.Length > 1 && string.Equals(reason.ToLower(), "pullrequest"))
            {
                System.Console.WriteLine("Checking Prod Version");
                var client = _factory.CreateClient();

                // Act
                var response = await client.GetAsync("/api/version");
                var content = await response.Content.ReadAsStringAsync();

                using (var prodClient = new HttpClient() { BaseAddress = new System.Uri("https://beta.amphoradata.com/") })
                {
                    var prodResponse = await prodClient.GetAsync("api/version");
                    var prodContent = await prodResponse.Content.ReadAsStringAsync();

                    // Assert
                    await AssertHttpSuccess(response);
                    var thisVersion = ApiVersionIdentifier.FromSemver(content);
                    await AssertHttpSuccess(prodResponse);
                    var prodVersion = ApiVersionIdentifier.FromSemver(prodContent);

                    Assert.True(thisVersion.Major >= prodVersion.Major);
                    if (thisVersion.Major == prodVersion.Major)
                    {
                        // MAJOR hasn't been incremented.
                        Assert.True(thisVersion.Minor >= prodVersion.Minor);
                        if (thisVersion.Minor == prodVersion.Minor)
                        {
                            // MINOR hasn't been incremented
                            Assert.True(thisVersion.Patch >= prodVersion.Patch);
                        }
                    }

                    // Assert at least one must be strictly greater than
                    Assert.True(thisVersion.Major > prodVersion.Major || thisVersion.Minor > prodVersion.Minor || thisVersion.Patch > prodVersion.Patch);
                }
            }
            else
            {
                System.Console.WriteLine("Not checking version");
            }
        }

        [Fact]
        public async Task WhenEnvironmentVariable_ChangeLogExists()
        {
            var reason = System.Environment.GetEnvironmentVariable("BUILD_REASON");
            var client = _factory.CreateClient();
            if (reason != null && reason.Length > 1 && string.Equals(reason.ToLower(), "pullrequest"))
            {
                // Get the current version
                var response = await client.GetAsync("/api/version");
                var content = await response.Content.ReadAsStringAsync();
                var version = ApiVersionIdentifier.FromSemver(content);

                // get the changelog for this version
                var getVersionResponse = await client.GetAsync($"Changelog/Detail?version={version.ToSemver()}");
                await AssertHttpSuccess(getVersionResponse);
            }
            else
            {
                System.Console.WriteLine("Not checking changelog");
            }
        }

        [Fact]
        public async Task VersionFileGeneration()
        {
            // used in the build pipe for smooth rollout of patch versions
            var versionFilePath = $"{System.IO.Directory.GetCurrentDirectory()}/version.txt";
            System.Console.WriteLine(versionFilePath);
            var version = ApiVersion.CurrentVersion;
            string[] lines = { version.ToSemver() };
            await System.IO.File.WriteAllLinesAsync(versionFilePath, lines);

            var fileVersion = await System.IO.File.ReadAllLinesAsync(versionFilePath);
            Assert.Equal(version.ToSemver(), fileVersion.FirstOrDefault());
        }
    }
}