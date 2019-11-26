using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class BasicWebServer
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public BasicWebServer(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Profiles/Account/Edit")]
        [InlineData("/Market")]
        [InlineData("/Amphorae")]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Detail")]
        [InlineData("/Profiles/Account/Login")]
        [InlineData("/Profiles/Account/Register")]
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

        [Fact]
        public async Task Get_NotAPage_Error()
        {
            // Arrange
            var url = "dsksdljkvn/sdjbskvj/";
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.False(response.IsSuccessStatusCode); // Status Code 200-299

        }


    }
}