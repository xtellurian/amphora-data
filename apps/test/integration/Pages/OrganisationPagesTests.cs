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
        [InlineData("")]
        [InlineData("?Name=kittens")]
        [InlineData("Create")]
        [InlineData("Detail")]
        [InlineData("Edit")]
        [InlineData("TermsOfUse")]
        [InlineData("TermsOfUse/Create")]
        public async Task CanLoadOrganisationsPage(string pageName)
        {
            var path = $"/Organisations/{pageName}";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var response = await adminClient.GetAsync(path);
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Credits")]
        [InlineData("Debits")]
        [InlineData("Invite")]
        [InlineData("Invoices")]
        [InlineData("Members")]
        [InlineData("Plan")]
        [InlineData("SelectPlan")]
        public async Task CanLoadAccountPage(string pageName)
        {
            var path = $"/Organisations/Account/{pageName}";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var response = await adminClient.GetAsync(path);
            await AssertHttpSuccess(response, path);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}