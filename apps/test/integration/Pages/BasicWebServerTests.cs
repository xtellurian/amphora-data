using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Terms;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(ApiFixtureCollection))]
    public class BasicWebServer : WebAppIntegrationTestBase
    {
        public BasicWebServer(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Home/Values")]
        [InlineData("/Home/Error")]
        [InlineData("/Home/StatusCode?code=404")]
        public async Task Get_UnAuthenticated_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString().ToLower());
        }

        [Theory]
        [InlineData("/Home/Values")]
        [InlineData("/Home/Error")]
        [InlineData("/DataRequests/Create")]
        [InlineData("/Changelog")]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Discover")]
        [InlineData("/Discover/DataRequests")]
        [InlineData("/Amphorae")]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Detail")]
        [InlineData("/AccessDenied")]
        public async Task Get_Authenticated_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString().ToLower());
        }

        [Fact]
        public async Task Get_Authenticated_TnCPages()
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var tou = new TermsOfUse();
            tou.Name = System.Guid.NewGuid().ToString();
            tou.Contents = System.Guid.NewGuid().ToString();

            var result = await client.PostAsJsonAsync($"api/TermsOfUse", tou);
            tou = JsonConvert.DeserializeObject<TermsOfUse>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(tou.Id);
            // Act
            var response = await client.GetAsync($"Amphorae/TermsOfUse/Detail?id={tou.Id}");
            await AssertHttpSuccess(response);
            var otherResponse = await otherClient.GetAsync($"Amphorae/TermsOfUse/Detail?id={tou.Id}");
            await AssertHttpSuccess(otherResponse);
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