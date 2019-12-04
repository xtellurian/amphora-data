using System.Threading.Tasks;
using Amphora.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class BasicWebServer: IntegrationTestBase
    {
        public BasicWebServer(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Values")]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Profiles/Account/Edit")]
        [InlineData("/Market")]
        [InlineData("/Amphorae")]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Detail")]
        [InlineData("/Profiles/Account/Login")]
        [InlineData("/Profiles/Account/Register")]
        public async Task Get_UnAuthenticated_EndpointsReturnSuccessAndCorrectContentType(string url)
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

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Values")]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Profiles/Account/Edit")]
        [InlineData("/Market")]
        [InlineData("/Amphorae")]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Detail")]
        public async Task Get_Authenticated_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

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