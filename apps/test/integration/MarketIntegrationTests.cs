using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class MarketIntegrationTests : WebAppIntegrationTestBase
    {
        public MarketIntegrationTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task TopAndSkip()
        {
            // Arrange
            var client = await GetUserAsync();

            var amphorae = new List<DetailedAmphora>();
            int i = 0;
            while (i < 10)
            {
                var a = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id, nameof(TopAndSkip));
                var createResponse = await client.Http.PostAsJsonAsync("api/amphorae", a);
                var createResponseContent = await createResponse.Content.ReadAsStringAsync();
                await AssertHttpSuccess(createResponse);
                a = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);
                amphorae.Add(a);
                i++;
            }

            // try reindex
            var indexRes = await client.Http.PostAsJsonAsync("api/search/indexers", new object());
            if (indexRes.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // wait 180 seconds and try again
                System.Console.WriteLine("Indexer failed. Waiting 180 seconds...");
                await Task.Delay(180 * 1000);
                indexRes = await client.Http.PostAsJsonAsync("api/search/indexers", new object());
            }

            await AssertHttpSuccess(indexRes);
            // how do we get this to index first?
            var top = 2;
            var k = 0;
            var res = await client.Http.GetAsync($"api/market/search?query=&top={top}&skip={k}");
            var content = await res.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<BasicAmphora>>(content);
        }
    }
}