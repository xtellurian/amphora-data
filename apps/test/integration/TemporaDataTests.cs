using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraSignalDataTests: IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public AmphoraSignalDataTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task UploadSignalTo_MissingAmphora(string url)
        {
             // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            var id = System.Guid.NewGuid();
            var json = Helpers.BadJsonLibrary.GetJson(Helpers.BadJsonLibrary.DiverseTypesKey);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync($"{url}/{id}/signals", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound , response.StatusCode);
        }
    }
}