using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Healthz
{
    [Collection(nameof(ApiFixtureCollection))]
    public class ApiHealthzTests : WebAppIntegrationTestBase
    {
        public ApiHealthzTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task CanGetHealthz()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/healthz");
            // Assert
            await AssertHttpSuccess(response);
        }
    }
}