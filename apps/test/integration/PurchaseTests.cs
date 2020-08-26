using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.AccessControls;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Api.Models.Dtos.Amphorae;
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
            var account1 = JsonConvert.DeserializeObject<AccountInformation>(accountContent);
            Assert.NotNull(account1);

            var purchaseRes = await client.PostAsJsonAsync($"api/Amphorae/{dto.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(purchaseRes);
            // get account again
            accountResponse = await client.GetAsync($"api/Organisations/{org.Id}/Account");
            accountContent = await accountResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(accountResponse);
            var account2 = JsonConvert.DeserializeObject<AccountInformation>(accountContent);
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
            var account1 = JsonConvert.DeserializeObject<AccountInformation>(accountContent);
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
        }

        [Fact]
        public async Task MultipleUsersInSameOrg_CantPurchaseTwice()
        {
            // going to need 3 users in 2 orgs
            var seller = await GetPersonaAsync(Personas.AmphoraAdmin);
            // make the buyers
            var buyer = await GetPersonaAsync(Personas.Standard);
            var buyer2 = await GetPersonaAsync(Personas.StandardTwo);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(seller.Organisation.Id);
            var createResponse = await seller.Http.PostAsJsonAsync("api/amphorae", amphora);
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // buyer 1 can purchase
            var purchase1Response = await buyer.Http.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purchase1Content = await purchase1Response.Content.ReadAsStringAsync();
            await AssertHttpSuccess(purchase1Response);

            // buyer 2 purchase should fail
            var purchase2Response = await buyer2.Http.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purchase2Content = await purchase2Response.Content.ReadAsStringAsync();
            Assert.False(purchase2Response.IsSuccessStatusCode);
        }
    }
}