using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class PurchaseTests : IntegrationTestBase
    {
        public PurchaseTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {

        }
        [Fact]
        public async Task PurchasingAmphora_DeductsFromBalance()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(PurchasingAmphora_DeductsFromBalance));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            accountResponse.EnsureSuccessStatusCode();
            var account1 = JsonConvert.DeserializeObject<Account>(accountContent);
            Assert.NotNull(account1);

            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new {});
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            purchaseRes.EnsureSuccessStatusCode();
            // get account again
            accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            accountContent = await accountResponse.Content.ReadAsStringAsync();
            accountResponse.EnsureSuccessStatusCode();
            var account2 = JsonConvert.DeserializeObject<Account>(accountContent);
            // get balance is deducted
            Assert.Equal(account1.Balance - dto.Price, account2.Balance);
        }

    }
}