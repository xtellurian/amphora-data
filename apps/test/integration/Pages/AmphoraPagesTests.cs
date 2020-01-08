using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class AmphoraPagesTests : IntegrationTestBase
    {
        public AmphoraPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Delete")]
        [InlineData("/Amphorae/Detail")]
        [InlineData("/Amphorae/Edit")]
        [InlineData("/Amphorae/File")]
        [InlineData("/Amphorae/Forbidden")]
        [InlineData("/Amphorae/Index")]
        [InlineData("/Amphorae/Invite")]
        public async Task CanLoadPage_AsOwner(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage_AsOwner));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());
            var id = dto.Id;

            var response = await adminClient.GetAsync($"{path}?id={id}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("/Amphorae/Purchase")]
        public async Task CanLoadPage_AsOther(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage_AsOther));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());
            var id = dto.Id;

            var response = await otherClient.GetAsync($"{path}?id={id}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);

            await DestroyOrganisationAsync(otherClient, otherOrg);
            await DestroyUserAsync(otherClient, otherUser);
        }

        [Theory]
        [InlineData("/Amphorae/StaticMap")]
        public async Task CanLoadImage(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadImage));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());
            var id = dto.Id;

            var response = await adminClient.GetAsync($"{path}?id={id}");
            response.EnsureSuccessStatusCode();
            Assert.Contains("image/", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}