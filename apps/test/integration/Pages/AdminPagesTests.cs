using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AdminPagesTests : WebAppIntegrationTestBase
    {
        public AdminPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task CanLoadPage()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync("example.com");

            var pagePaths = new List<string>
            {
                "/Admin/Index",
                "/Admin/Dashboard",
                "/Admin/Accounts/Index",
                "/Admin/TermsOfUse",
                "/Admin/TermsOfUse/Create",
            };

            foreach (var path in pagePaths)
            {
                var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage));
                adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
                await AssertHttpSuccess(createResponse);

                var response = await adminClient.GetAsync($"{path}");
                await AssertHttpSuccess(response, path);
                Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

                var otherResponse = await otherClient.GetAsync(path);
                otherResponse.IsSuccessStatusCode.Should().BeFalse("because the user isn't global admin");
            }

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
            otherResponse.IsSuccessStatusCode.Should().BeFalse("because the user isn't global admin");

            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}