using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Permissions;
using Amphora.Common.Models.Permissions;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class AmphoraRestrictionsTests : IntegrationTestBase
    {
        public AmphoraRestrictionsTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task RestrictedAmphora_OtherCannotPurchase()
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
            var restriction = new Restriction { TargetOrganisationId = otherOrg.Id, Kind = RestrictionKind.Deny };
            var restrictionResponse = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/Restrictions", restriction);
            var restrictionResponseContent = await restrictionResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(restrictionResponse);
            restriction = JsonConvert.DeserializeObject<Restriction>(restrictionResponseContent);

            // check the other cannot purchase or access
            var purchaseRes = await otherClient.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            var purhcaseContent = await purchaseRes.Content.ReadAsStringAsync();
            Assert.False(purchaseRes.IsSuccessStatusCode, "Error when purchasing restricted Amphora");

            // delete the restriction
            var deleteRes = await adminClient.DeleteAsync($"api/amphorae/{amphora.Id}/Restrictions/{restriction.Id}");
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
    }
}