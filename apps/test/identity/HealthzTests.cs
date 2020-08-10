using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    [Trait("Category", "Identity")]
    public class IdentityHealthzTests : IdentityIntegrationTestBase
    {
        public IdentityHealthzTests(WebApplicationFactory<Amphora.Identity.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task CanGetHealthz()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/healthz");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}