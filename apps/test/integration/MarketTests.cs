using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class MarketTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public MarketTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {

        }

        [Fact]
        public async Task MarketSearch_EmptyString()
        {
            var (client, user, org) = await base.NewOrgAuthenticatedClientAsync();
            var term = string.Empty;
            var response = await client.GetAsync("api/market?query='{term}'");

            response.EnsureSuccessStatusCode();

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }
    }
}