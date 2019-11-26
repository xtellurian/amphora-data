using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]

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
            response.EnsureSuccessStatusCode();
            var version = ApiVersionIdentifier.FromSemver(content);
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
                    response.EnsureSuccessStatusCode();
                    var thisVersion = ApiVersionIdentifier.FromSemver(content);
                    prodResponse.EnsureSuccessStatusCode();
                    var prodVersion = ApiVersionIdentifier.FromSemver(prodContent);

                    Assert.True(thisVersion.Major >= prodVersion.Major);
                    Assert.True(thisVersion.Minor >= prodVersion.Minor);
                    Assert.True(thisVersion.Patch >= prodVersion.Patch);
                    // Assert at least one must be strictly greater than
                    Assert.True(thisVersion.Major > prodVersion.Major || thisVersion.Minor > prodVersion.Minor || thisVersion.Patch > prodVersion.Patch);
                }
            }
            else
            {
                System.Console.WriteLine("Not checking version");
            }
        }
    }
}