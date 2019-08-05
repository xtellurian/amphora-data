using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class MyAmphoraeTests: IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public MyAmphoraeTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("myAmphorae", "AmphoraData")]
        public async Task Get_MyAmphorae_ByOrgId(string url, string orgId)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.AmphoraLibrary.GetValidAmphora();
            a.OrgId = orgId;
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PutAsync("api/amphorae", requestBody);

            // Act
            var response = await client.GetAsync($"{url}?orgId={orgId}");
            response.EnsureSuccessStatusCode();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

        }
    }
}