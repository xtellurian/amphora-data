using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class OrganisationPagesTests : IntegrationTestBase
    {
        public OrganisationPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Organisations/AcceptInvitation")]
        [InlineData("/Organisations/Create")]
        [InlineData("/Organisations/Detail")]
        [InlineData("/Organisations/Edit")]
        [InlineData("/Organisations/Index")]
        [InlineData("/Organisations/Invite")]
        [InlineData("/Organisations/Members")]
        // [InlineData("/Organisations/SetRole")] // doesn't work, needs extra params
        [InlineData("/Organisations/TermsAndConditions/Index")]
        public async Task CanLoadPage(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var response = await adminClient.GetAsync(path);
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}