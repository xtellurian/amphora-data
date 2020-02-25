using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Permissions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Organisations
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class OrganisationRestrictionsTests : IntegrationTestBase
    {
        public OrganisationRestrictionsTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateAndDeleteRestriction()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherOrgClient, otherOrgUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var restriction = new Restriction();
            restriction.TargetOrganisationId = otherOrg.Id;
            restriction.Kind = Common.Models.Permissions.RestrictionKind.Deny;

            var result = await adminClient.PostAsJsonAsync($"api/Organisations/{adminOrg.Id}/Restrictions", restriction);
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var dto = JsonConvert.DeserializeObject<Restriction>(contents);
            Assert.Equal(restriction.TargetOrganisationId, dto.TargetOrganisationId);
            Assert.Equal(restriction.Kind, dto.Kind);

            var response = await adminClient.DeleteAsync($"api/Organisations/{adminOrg.Id}/Restrictions/{restriction.TargetOrganisationId}");
            await AssertHttpSuccess(response);
        }
    }
}