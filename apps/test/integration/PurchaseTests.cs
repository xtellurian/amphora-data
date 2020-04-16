using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.AccessControls;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class PurchaseTests : WebAppIntegrationTestBase
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
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(accountResponse);
            var account1 = JsonConvert.DeserializeObject<Account>(accountContent);
            Assert.NotNull(account1);

            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(purchaseRes);
            // get account again
            accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            accountContent = await accountResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(accountResponse);
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
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purchaseContent = await purchaseRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(purchaseRes);

            // get the amphora again from the server
            var res = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            await AssertHttpSuccess(res);
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
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(accountResponse);
            var account1 = JsonConvert.DeserializeObject<Account>(accountContent);
            Assert.NotNull(account1);

            // retrict this org
            var denyRule = OrganisationAccessRule.Deny(org.Id);
            var restrictRes = await adminClient.PostAsJsonAsync($"api/Amphorae/{dto.Id}/AccessControls/ForOrganisation", denyRule);
            var restrictContent = await restrictRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(restrictRes);

            // attempt to purchase
            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            Assert.False(purchaseRes.IsSuccessStatusCode);

            await DestroyAmphoraAsync(adminClient, dto.Id);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task MultipleUsersInSameOrg_CantPurchaseTwice()
        {
            // going to need 3 users in 2 orgs
            var (sellingClient, sellingUser, sellingOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(sellingOrg.Id);
            sellingClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await sellingClient.PostAsJsonAsync("api/amphorae", amphora);
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // make the buyers
            var (buyerClient1, buyer1, buyingOrg) = await NewOrgAuthenticatedClientAsync(planType: TeamPlan);
            var (buyerClient2, buyer2, buyingOrg2) = await GetNewClientInOrg(buyerClient1, buyingOrg);

            // buyer 1 can purchase
            var purchase1Response = await buyerClient1.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purchase1Content = await purchase1Response.Content.ReadAsStringAsync();
            await AssertHttpSuccess(purchase1Response);

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