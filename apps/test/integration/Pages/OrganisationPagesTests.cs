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
        [InlineData("TermsAndConditions")]
        [InlineData("TermsAndConditions/Create")]
        public async Task CanLoadOrganisationsPage(string pageName)
        {
            var path = $"/Organisatons/{pageName}";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var qString = $"?id={adminOrg.Id}";
            var response = await adminClient.GetAsync(path + qString);
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
            var qString = $"?id={adminOrg.Id}";
            var response = await adminClient.GetAsync(path + qString);
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}