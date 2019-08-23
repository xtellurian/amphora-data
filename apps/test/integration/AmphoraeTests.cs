using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraeTests: IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public AmphoraeTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("amphorae", "default")]
        public async Task Get_MyAmphorae_ByOrgId(string url, string orgId)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetValidAmphora();
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

        [Theory]
        [InlineData("r3gx2f77b", "r3gx2f77b", true)]
        [InlineData("r3gx2f77b", "r3gx2f", true)]
        [InlineData("r3gx2f77b", "r3g", true)]
        [InlineData("r3gx2f77b", "r4g", false)]
        [InlineData("r3gx2f77b", "skdjvlsv", false)]
        public async Task Get_QueryAmphoraByGeohash(string geoHash, string queryGeoHash, bool success)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetValidAmphora();
            a.GeoHash = geoHash;// set the geohash
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PutAsync("api/amphorae", requestBody);
            var amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(
                await createResponse.Content.ReadAsStringAsync());
            Assert.NotNull(amphora);
            Assert.NotNull(amphora.Id);

            // Act
            var response = await client.GetAsync($"api/amphorae?geoHash={queryGeoHash}");

            //Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var entities = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Amphora>>(responseContent);
            if(success)
            {
                Assert.Contains(entities, e => string.Equals(amphora.Id, e.Id));
            } 
            else
            {
                Assert.DoesNotContain(entities, e => string.Equals(amphora.Id, e.Id));
            }

        }
    }
}