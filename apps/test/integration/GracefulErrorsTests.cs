using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class GracefulErrorsTests: IntegrationTestBase
    {

        public GracefulErrorsTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_RandomByteArray_Unauthorized(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);

            // Act
            var response = await client.PostAsync(url, new ByteArrayContent(content));
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.DiverseTypesKey)]
        [InlineData("/api/amphorae", Helpers.BadJsonLibrary.BadlyFormedAmphoraKey)]
        public async Task Post_WeirdJson(string url, string key = "")
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var content = new StringContent(Helpers.BadJsonLibrary.GetJson(key), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            
        }
    }
}