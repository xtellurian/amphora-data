using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class ProfilePagesTests : IntegrationTestBase
    {
        public ProfilePagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Profiles/Account/Detail")]
        [InlineData("/Profiles/Account/Edit")]
        [InlineData("/Profiles/Account/UserMissing")]
        public async Task CanLoadPage(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var response = await adminClient.GetAsync(path);
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}