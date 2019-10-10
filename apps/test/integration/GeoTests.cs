using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Common.Models.AzureMaps;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class GeoTests: IntegrationTestBase
    {

        public GeoTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {
        }
        [Fact]
       public async Task FuzzySearchTests()
       {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            // Act
            var response = await client.GetAsync("/api/location/fuzzy?query=sydney");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<FuzzySearchResponse>(content);
            Assert.NotNull(obj);
            Assert.NotEmpty(obj.Results);

       }
    }
}