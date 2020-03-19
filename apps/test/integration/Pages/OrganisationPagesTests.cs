using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(ApiFixtureCollection))]
    public class OrganisationPagesTests : WebAppIntegrationTestBase
    {
        public OrganisationPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Organisations/Create")]
        [InlineData("/Organisations/Detail")]
        [InlineData("/Organisations/Edit")]
        [InlineData("/Organisations/Index")]
        [InlineData("/Organisations?Name=kittens")]
        // [InlineData("/Organisations/Join")] // TODO: needs an invitation
        [InlineData("/Organisations/Members")]
        [InlineData("/Organisations/TermsAndConditions")]
        [InlineData("/Organisations/TermsAndConditions/Create")]
        // [InlineData("/Organisations/SetRole")] // doesn't work, needs extra params
        [InlineData("/Organisations/TermsAndConditions/Index")]
        public async Task CanLoadPage(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var qString = $"?id={adminOrg.Id}";
            var response = await adminClient.GetAsync(path + qString);
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}