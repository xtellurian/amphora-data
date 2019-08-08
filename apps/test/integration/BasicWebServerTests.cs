using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class BasicWebServer
        : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public BasicWebServer(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Home/Settings")]
        [InlineData("/Profile")]
        [InlineData("/Market")]
        [InlineData("/Amphorae")]
        [InlineData("/Amphorae/create")]
        [InlineData("/Amphorae/detail")]
        [InlineData("/Temporae")]
        [InlineData("/Temporae/create")]
        [InlineData("/Temporae/detail")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}