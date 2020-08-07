using System.Threading.Tasks;
using Amphora.Tests.Setup;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    [Trait("Category", "Identity")]
    public class IdentityHealthzTests
    {
        private readonly InMemoryIdentityWebApplicationFactory factory;

        public IdentityHealthzTests(InMemoryIdentityWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CanGetHealthz()
        {
            // Arrange
            var client = factory.CreateClient();
            // Act
            var response = await client.GetAsync("/healthz");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}