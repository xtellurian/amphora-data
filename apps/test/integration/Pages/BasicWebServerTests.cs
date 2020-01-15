using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class BasicWebServer : IntegrationTestBase
    {
        public BasicWebServer(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Values")]
        [InlineData("/Home/Error")]
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
        [InlineData("/Home/Error")]
        [InlineData("/DataRequests/Create")]
        [InlineData("/Changelog")]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Profiles/Account/Edit")]
        [InlineData("/Market")] // TODO: depricate this link
        [InlineData("/Discover")]
        [InlineData("/Discover/DataRequests")]
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
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [InlineData("Organisations/TermsAndConditions")]
        [InlineData("Organisations/TermsAndConditions/Detail")]
        [Theory]
        public async Task Get_Authenticated_TnCPages(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var tnc = new TermsAndConditionsDto();
            tnc.Id = System.Guid.NewGuid().ToString();
            tnc.Name = System.Guid.NewGuid().ToString();
            tnc.Contents = System.Guid.NewGuid().ToString();

            var result = await client.PostAsJsonAsync($"api/Organisations/{org.Id}/TermsAndConditions", tnc);

            // Act
            var response = await client.GetAsync($"{url}?id={org.Id}&tncId={tnc.Id}");
            var otherResponse = await otherClient.GetAsync($"{url}?id={org.Id}&tncId={tnc.Id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            otherResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                otherResponse.Content.Headers.ContentType.ToString());
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