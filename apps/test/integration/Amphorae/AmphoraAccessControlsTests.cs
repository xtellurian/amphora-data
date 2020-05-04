using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.AccessControls;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraAccessControlsTests : WebAppIntegrationTestBase
    {
        public AmphoraAccessControlsTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task RestrictedAmphora_ForOrganisation_OtherCannotPurchase()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // restrict the other org
            var rule = OrganisationAccessRule.Deny(otherOrg.Id);
            var ruleResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/AccessControls/ForOrganisation", rule);
            var ruleResponseContent = await ruleResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(ruleResponse);
            rule = JsonConvert.DeserializeObject<OrganisationAccessRule>(ruleResponseContent);

            // check the other cannot purchase or access
            var purchaseRes = await otherClient.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            Assert.False(purchaseRes.IsSuccessStatusCode, "Error when purchasing restricted Amphora");

            // delete the access control rule
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/AccessControls/{rule.Id}");
            await AssertHttpSuccess(deleteRes);

            // now purchase should work
            var purchaseAgainRes = await otherClient.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            await AssertHttpSuccess(purchaseAgainRes);

            await DestroyAmphoraAsync(adminClient, amphora.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }

        [Fact]
        public async Task RestrictedAmphora_ForUser_OtherCannotPurchase()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // restrict the other org
            var rule = UserAccessRule.Deny(otherUser.UserName);
            var ruleResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/AccessControls/ForUser", rule);
            var ruleResponseContent = await ruleResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(ruleResponse);
            rule = JsonConvert.DeserializeObject<UserAccessRule>(ruleResponseContent);

            // check the other cannot purchase or access
            var purchaseRes = await otherClient.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            Assert.False(purchaseRes.IsSuccessStatusCode, "Error when purchasing restricted Amphora");

            // delete the access control rule
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/AccessControls/{rule.Id}");
            await AssertHttpSuccess(deleteRes);

            // now purchase should work
            var purchaseAgainRes = await otherClient.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            await AssertHttpSuccess(purchaseAgainRes);

            await DestroyAmphoraAsync(adminClient, amphora.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }

        [Fact]
        public async Task AccessControls_ForOrganisation_CanBeListedFromApi()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // restrict the other org
            var rule = OrganisationAccessRule.Deny(otherOrg.Id);
            var ruleResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/AccessControls/ForOrganisation", rule);
            var ruleResponseContent = await ruleResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(ruleResponse);
            rule = JsonConvert.DeserializeObject<OrganisationAccessRule>(ruleResponseContent);

            // get the rules
            var rulesResponse = await adminClient.GetAsync($"api/amphorae/{amphora.Id}/AccessControls/ForOrganisation");
            await AssertHttpSuccess(rulesResponse);
            var res = JsonConvert.DeserializeObject<List<OrganisationAccessRule>>(await rulesResponse.Content.ReadAsStringAsync());
            Assert.NotEmpty(res);
            Assert.Equal(rule.Id, res[0].Id);

            // delete the rule
            // delete the access control rule
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/AccessControls/{rule.Id}");
            await AssertHttpSuccess(deleteRes);

            await DestroyAmphoraAsync(adminClient, amphora.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }

        [Fact]
        public async Task AccessControls_ForUser_CanBeListedFromApi()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // restrict the other org
            var rule = UserAccessRule.Deny(otherUser.UserName);
            var ruleResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/AccessControls/ForUser", rule);
            var ruleResponseContent = await ruleResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(ruleResponse);
            rule = JsonConvert.DeserializeObject<UserAccessRule>(ruleResponseContent);
            Assert.NotNull(rule.Id);

            // get the rules
            var rulesResponse = await adminClient.GetAsync($"api/amphorae/{amphora.Id}/AccessControls/ForUser");
            await AssertHttpSuccess(rulesResponse);
            var res = JsonConvert.DeserializeObject<List<UserAccessRule>>(await rulesResponse.Content.ReadAsStringAsync());
            Assert.NotEmpty(res);
            Assert.Equal(rule.Id, res[0].Id);

            // delete the rule
            // delete the access control rule
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/AccessControls/{rule.Id}");
            await AssertHttpSuccess(deleteRes);

            await DestroyAmphoraAsync(adminClient, amphora.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }

        [Fact]
        public async Task AccessControls_AllowAll_DataCanBeAccessedByAnyone()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // and add a file to the amphora
            var file = System.Guid.NewGuid().ToString();
            var generator = new Helpers.RandomGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await adminClient.PutAsync($"api/amphorae/{amphora.Id}/files/{file}", requestBody);
            await AssertHttpSuccess(uploadResponse);

            // allow all to access
            var rule = AllAccessRule.Allow();
            var ruleResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/AccessControls/ForAll", rule);
            var ruleResponseContent = await ruleResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(ruleResponse);
            rule = JsonConvert.DeserializeObject<AllAccessRule>(ruleResponseContent);
            Assert.NotNull(rule.Id);

            // get the rule
            var rulesResponse = await adminClient.GetAsync($"api/amphorae/{amphora.Id}/AccessControls/ForAll");
            await AssertHttpSuccess(rulesResponse);
            var res = JsonConvert.DeserializeObject<AllAccessRule>(await rulesResponse.Content.ReadAsStringAsync());
            Assert.Equal(rule.Id, res.Id);
            Assert.Equal(rule.AllowOrDeny, res.AllowOrDeny);
            Assert.Equal(rule.Priority, res.Priority);

            // check other can access data
            var getFileResponse = await otherClient.GetAsync($"api/amphorae/{amphora.Id}/files/{file}");
            await AssertHttpSuccess(getFileResponse);

            // delete the rule
            // delete the access control rule
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/AccessControls/{rule.Id}");
            await AssertHttpSuccess(deleteRes);

            // check other now CANT access data
            getFileResponse = await otherClient.GetAsync($"api/amphorae/{amphora.Id}/files/{file}");
            Assert.False(getFileResponse.IsSuccessStatusCode);

            await DestroyAmphoraAsync(adminClient, amphora.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }
    }
}