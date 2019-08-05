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
        [InlineData("/api/amphorae")]
        public async Task Post_RandomByteArray_MethodNotAllowed(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);

            // Act
            var response = await client.PostAsync(url, new ByteArrayContent(content));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.DiverseTypesKey)]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.BadlyFormedAmphoraKey)]
        public async Task Post_WeirdJson(string url, string key = "")
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent(Helpers.BadJsonLibrary.GetJson(key), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.DiverseTypesKey)]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.BadlyFormedAmphoraKey)]
        public async Task Put_WeirdJson(string url, string key = "")
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent(Helpers.BadJsonLibrary.GetJson(key), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync(url, content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            
        }
    }
}