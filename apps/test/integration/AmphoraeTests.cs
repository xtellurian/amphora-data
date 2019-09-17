using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
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
        [InlineData("api/search/amphorae/byOrganisation")]
        public async Task GetAmphorae_ByOrgId_AsUser(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (client, user, org) = await base.GetNewClientInOrg(adminClient, adminOrg);
            var a = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId, nameof(GetAmphorae_ByOrgId_AsUser));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            a = JsonConvert.DeserializeObject<AmphoraExtendedModel>(createResponseContent);

            // Act
            var response = await client.GetAsync($"{url}?orgId={user.OrganisationId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<AmphoraModel>>(content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
           //  Assert.Contains(amphorae, b => string.Equals(b.Id, a.Id));

            await DeleteAmphora(adminClient, a.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);

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