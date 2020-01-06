using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class SearchTests : IntegrationTestBase
    {
        public SearchTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("api/search/amphorae/byOrganisation")]
        public async Task SearchAmphorae_ByOrgId_AsUser(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (client, user, org) = await GetNewClientInOrg(adminClient, adminOrg);
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(SearchAmphorae_ByLocation));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            a = JsonConvert.DeserializeObject<AmphoraExtendedDto>(createResponseContent);

            // Act
            var response = await client.GetAsync($"{url}?orgId={user.OrganisationId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<AmphoraExtendedDto>>(content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
                                                //  Assert.Contains(amphorae, b => string.Equals(b.Id, a.Id));

            await DestroyAmphoraAsync(adminClient, a.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("api/search/amphorae/byLocation")]
        public async Task SearchAmphorae_ByLocation(string url)
        {
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var rnd = new Random();
            var lat = rnd.Next(90);
            var lon = rnd.Next(90);
            var term = string.Empty;
            // let's create an amphorae
            var a = EntityLibrary.GetAmphoraDto(org.Id, nameof(SearchAmphorae_ByLocation));
            a.Lat = lat;
            a.Lon = lon;
            var content = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync("api/amphorae", content);
            a = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            var response = await client.GetAsync($"{url}?lat={lat}&lon={lon}&dist=10");
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseList = JsonConvert.DeserializeObject<List<AmphoraExtendedDto>>(responseContent);
            // Assert.Contains(responseList, b => string.Equals(b.Id, a.Id)); // how to wait for the indexer to run?

            await DestroyAmphoraAsync(client, a.Id);
            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }
    }
}