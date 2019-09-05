using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraeTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {

        public AmphoraeTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("api/amphorae", "default")]
        public async Task Get_MyAmphorae_ByOrgId(string url, string orgId)
        {
            // Arrange
            var (client, user, org) = await GetAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphora(org.OrganisationId, description: nameof(Get_MyAmphorae_ByOrgId));
            a.OrganisationId = orgId;
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            a = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(createResponseContent);

            // Act
            var response = await client.GetAsync($"{url}?orgId={orgId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Amphora>>(content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Contains(amphorae, b => string.Equals(b.Id, a.Id));

            await DeleteAmphora(client, a.Id);
            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);

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
            var (client, user, org) = await GetAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(org.OrganisationId, description: nameof(Get_QueryAmphoraByGeohash));
            a.GeoHash = geoHash;// set the geohash
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync("api/amphorae", requestBody);
            var amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(
                await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            Assert.NotNull(amphora);
            Assert.NotNull(amphora.Id);

            // Act
            var response = await client.GetAsync($"api/amphorae?geoHash={queryGeoHash}");

            //Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var entities = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Amphora>>(responseContent);
            if (success)
            {
                Assert.Contains(entities, e => string.Equals(amphora.Id, e.Id));
            }
            else
            {
                Assert.DoesNotContain(entities, e => string.Equals(amphora.Id, e.Id));
            }

            await DeleteAmphora(client, amphora.Id);
            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var res = await client.DeleteAsync($"/api/amphorae/{id}");
            res.EnsureSuccessStatusCode();
            var response = await client.GetAsync($"api/amphorae/{id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}