using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
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
            await AssertHttpSuccess(createResponse);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            // Act
            var response = await client.GetAsync($"{url}?orgId={user.OrganisationId}");
            await AssertHttpSuccess(response);
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<DetailedAmphora>>(content);

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
            a = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());
            await AssertHttpSuccess(createResponse);
            var response = await client.GetAsync($"{url}?lat={lat}&lon={lon}&dist=10");
            var responseContent = await response.Content.ReadAsStringAsync();
            await AssertHttpSuccess(response);

            var responseList = JsonConvert.DeserializeObject<List<DetailedAmphora>>(responseContent);
            // Assert.Contains(responseList, b => string.Equals(b.Id, a.Id)); // how to wait for the indexer to run?

            await DestroyAmphoraAsync(client, a.Id);
            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }

        [Fact]
        public async Task CanSearchOrganisations()
        {
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            // now we have an org, it should show up in the search.

            var searchRes = await client.GetAsync($"api/search/organisations?term={org.Name}");
            var content = await searchRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(searchRes);
            var orgs = JsonConvert.DeserializeObject<List<Organisation>>(content);

            Assert.NotNull(orgs);
            // Assert.NotEmpty(orgs);
            // Assert.Contains(orgs, _ => _.Id == org.Id);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }
    }
}