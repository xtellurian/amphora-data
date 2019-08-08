using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class TemporaDataTests: IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public TemporaDataTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/temporae")]
        public async Task UploadTo_MissingTempora(string url)
        {
             // Arrange
            var client = _factory.CreateClient();
            var id = System.Guid.NewGuid();
            var json = Helpers.BadJsonLibrary.GetJson(Helpers.BadJsonLibrary.DiverseTypesKey);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync($"{url}/{id}/upload", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound , response.StatusCode);
        }

        [Theory]
        [InlineData("/api/temporae")]
        public async Task DownloadFrom_MissingTempora(string url)
        {
             // Arrange
            var client = _factory.CreateClient();
            var id = System.Guid.NewGuid();

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync($"{url}/{id}/download");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound , response.StatusCode);
        }
    }
}