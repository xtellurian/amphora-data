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
    public class SearchTests : WebAppIntegrationTestBase
    {
        public SearchTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("api/search/amphorae/byOrganisation")]
        public async Task SearchAmphorae_ByOrgIdAsUser_ForTeamPlan(string url)
        {
            // Arrange
            var client = await GetPersonaAsync();
            var otherClient = await GetPersonaAsync(Personas.StandardTwo);
            var a = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id);
            var createResponse = await client.Http.PostAsJsonAsync("api/amphorae", a);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            // Act
            var response = await otherClient.Http.GetAsync($"{url}?orgId={client.Organisation.Id}");
            await AssertHttpSuccess(response);
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<DetailedAmphora>>(content);
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
        }
    }
}