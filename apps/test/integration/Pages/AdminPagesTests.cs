using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AdminPagesTests : IntegrationTestBase
    {
        public AdminPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Admin/Index")]
        [InlineData("/Admin/Dashboard")]
        [InlineData("/Admin/Purchases")]
        [InlineData("/Admin/Accounts/Index")]
        public async Task CanLoadPage(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync("example.com");

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            await AssertHttpSuccess(createResponse);

            var response = await adminClient.GetAsync($"{path}");
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var otherResponse = await otherClient.GetAsync(path);
            Assert.False(otherResponse.IsSuccessStatusCode); // other is not authorized
            Assert.Equal(System.Net.HttpStatusCode.NotFound, otherResponse.StatusCode); // other will not find the page

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("/Admin/Accounts/Detail")]
        public async Task CanLoadPage_OrganisationSpecific(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync("example.com");

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            await AssertHttpSuccess(createResponse);

            var response = await adminClient.GetAsync($"{path}?id={otherOrg.Id}");
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            // ensure other can't access it
            var otherResponse = await otherClient.GetAsync($"{path}?id={otherOrg.Id}");
            Assert.False(otherResponse.IsSuccessStatusCode); // other is not authorized
            Assert.Equal(System.Net.HttpStatusCode.NotFound, otherResponse.StatusCode); // other is not authorized

            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}