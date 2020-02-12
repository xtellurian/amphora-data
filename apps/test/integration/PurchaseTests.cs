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
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            accountResponse.EnsureSuccessStatusCode();
            var account1 = JsonConvert.DeserializeObject<Account>(accountContent);
            Assert.NotNull(account1);

            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            purchaseRes.EnsureSuccessStatusCode();
            // get account again
            accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            accountContent = await accountResponse.Content.ReadAsStringAsync();
            accountResponse.EnsureSuccessStatusCode();
            var account2 = JsonConvert.DeserializeObject<Account>(accountContent);
            // get balance is deducted
            Assert.Equal(account1.Balance - dto.Price, account2.Balance);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task PurchasingAmphora_IncrementsPurchaseCount()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(PurchasingAmphora_DeductsFromBalance));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purchaseContent = await purchaseRes.Content.ReadAsStringAsync();
            purchaseRes.EnsureSuccessStatusCode();

            // get the amphora again from the server
            var res = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            res.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await res.Content.ReadAsStringAsync());

            Assert.NotNull(dto.PurchaseCount);
            Assert.True(dto.PurchaseCount > 0);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CantPurchaseWhenRestricted()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(PurchasingAmphora_DeductsFromBalance));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            accountResponse.EnsureSuccessStatusCode();
            var account1 = JsonConvert.DeserializeObject<Account>(accountContent);
            Assert.NotNull(account1);

            // retrict this org
            var restriction = new Restriction(org.Id, Common.Models.Permissions.RestrictionKind.Deny);
            var restrictRes = await adminClient.PostAsJsonAsync($"api/Organisations/{adminOrg.Id}/Restrictions", restriction);
            var restrictContent = await restrictRes.Content.ReadAsStringAsync();
            restrictRes.EnsureSuccessStatusCode();

            // attempt to purchase
            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            Assert.False(purchaseRes.IsSuccessStatusCode);

            await DestroyAmphoraAsync(adminClient, dto.Id);

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
        }

        [Fact]
        public async Task MultipleUsersInSameOrg_CantPurchaseTwice()
        {
            // going to need 3 users in 2 orgs
            var (sellingClient, sellingUser, sellingOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(sellingOrg.Id);
            sellingClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await sellingClient.PostAsJsonAsync("api/amphorae", amphora);
            createResponse.EnsureSuccessStatusCode();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // make the buyers
            var (buyerClient1, buyer1, buyingOrg) = await NewOrgAuthenticatedClientAsync();
            var (buyerClient2, buyer2, buyingOrg2) = await GetNewClientInOrg(buyerClient1, buyingOrg);

            // buyer 1 can purchase
            var purchase1Response = await buyerClient1.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purchase1Content = await purchase1Response.Content.ReadAsStringAsync();
            Assert.True(purchase1Response.IsSuccessStatusCode);

            // buyer 2 purchase should fail
            var purchase2Response = await buyerClient2.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purchase2Content = await purchase2Response.Content.ReadAsStringAsync();
            Assert.False(purchase2Response.IsSuccessStatusCode);

            await DestroyAmphoraAsync(sellingClient, amphora.Id);

            await DestroyOrganisationAsync(sellingClient, sellingOrg);
            await DestroyUserAsync(sellingClient, sellingUser);
            await DestroyUserAsync(buyerClient2, buyer2);
            await DestroyOrganisationAsync(buyerClient1, buyingOrg);
            await DestroyUserAsync(buyerClient1, buyer1);
        }
    }
}