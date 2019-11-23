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
            var check = System.Environment.GetEnvironmentVariable("AMPHORA_CHECK_VERSION");
            if(check != null && check.Length > 1 )
            {
                var client = _factory.CreateClient();

                // Act
                var response = await client.GetAsync("/api/version");
                var content = await response.Content.ReadAsStringAsync();

                var prodResponse = await client.GetAsync("https://beta.amphoradata.com/api/version");
                var prodContent = await prodResponse.Content.ReadAsStringAsync();

                // Assert
                response.EnsureSuccessStatusCode();
                var thisVersion = ApiVersionIdentifier.FromSemver(content);
                prodResponse.EnsureSuccessStatusCode();
                var prodVersion = ApiVersionIdentifier.FromSemver(prodContent);

                Assert.True(thisVersion.Major >= prodVersion.Major);
                Assert.True(thisVersion.Minor >= prodVersion.Minor);
                Assert.True(thisVersion.Patch >= prodVersion.Patch);
            }
        }
    }
}