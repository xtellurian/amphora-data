using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class GracefulErrorsTests
        : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public GracefulErrorsTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/api/amphorae")]
        public async Task Post_RandomByteArray(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);

            // Act
            var response = await client.PostAsync(url, new ByteArrayContent(content));

            // Assert
            response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/api/amphorae")]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.DiverseTypesKey)]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.DiverseTypesKey)]
        public async Task Post_WeirdJson(string url, string key = "")
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent(Helpers.BadJsonLibrary.GetJson(key), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        }
    }
}