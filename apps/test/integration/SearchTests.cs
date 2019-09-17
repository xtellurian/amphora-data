using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class SearchTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public SearchTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {

        }

     

        [Theory]
        [InlineData("api/search/amphorae/byLocation")]
        public async Task SearchAmphorae_ByLocation(string url)
        {
            var (client, user, org) = await base.NewOrgAuthenticatedClientAsync();
            var rnd = new Random();
            var location = new GeoLocation(rnd.Next(90), rnd.Next(90));
            var term = string.Empty;
            // let's create an amphorae
            var a = EntityLibrary.GetAmphora(org.OrganisationId, nameof(SearchAmphorae_ByLocation));
            a.GeoLocation = location;
            var content = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync("api/amphorae", content);
            a = JsonConvert.DeserializeObject<AmphoraExtendedModel>(await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            var response = await client.GetAsync($"{url}?lat={location.Lat()}&lon={location.Lon()}&dist=10");
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseList = JsonConvert.DeserializeObject<List<AmphoraModel>>(responseContent);
            // Assert.Contains(responseList, b => string.Equals(b.Id, a.Id)); // how to wait for the indexer to run?

            await DestroyAmphoraAsync(client, a.AmphoraId);
            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }
    }
}