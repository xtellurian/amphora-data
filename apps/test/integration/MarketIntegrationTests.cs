using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class MarketIntegrationTests : IntegrationTestBase
    {
        public MarketIntegrationTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task TopAndSkip()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (client, user, org) = await GetNewClientInOrg(adminClient, adminOrg);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var amphorae = new List<DetailedAmphora>();
            int i = 0;
            while (i < 10)
            {
                var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(TopAndSkip));
                var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", a);
                var createResponseContent = await createResponse.Content.ReadAsStringAsync();
                createResponse.EnsureSuccessStatusCode();
                a = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);
                amphorae.Add(a);
                i++;
            }

            // try reindex
            var indexRes = await adminClient.PostAsJsonAsync("api/search/indexers", new object());
            if (indexRes.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // wait 180 seconds and try again
                System.Console.WriteLine("Indexer failed. Waiting 180 seconds...");
                await Task.Delay(180 * 1000);
                indexRes = await adminClient.PostAsJsonAsync("api/search/indexers", new object());
            }

            indexRes.EnsureSuccessStatusCode();
            // how do we get this to index first?
            var top = 2;
            var k = 0;
            var res = await adminClient.GetAsync($"api/market/search?query=&top={top}&skip={k}");
            var content = await res.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<AmphoraDto>>(content);
            // Assert.Equal(top, list.Count); // FIXME

            foreach (var x in amphorae)
            {
                await DestroyAmphoraAsync(adminClient, x.Id);
            }

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}