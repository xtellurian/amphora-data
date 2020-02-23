using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class IdentityTests : IntegrationTestBase
    {
        public IdentityTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Profiles/Account/Login")]
        [InlineData("/Profiles/Account/Logout")]
        [InlineData("/Profiles/Account/Register")]
        public async Task Get_IdentityPage(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}