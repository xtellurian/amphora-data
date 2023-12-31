using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Pages
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraPagesTests : WebAppIntegrationTestBase
    {
        public AmphoraPagesTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Amphorae/Create")]
        [InlineData("/Amphorae/Delete")]
        [InlineData("/Amphorae/Detail")]
        [InlineData("/Amphorae/Edit")]
        [InlineData("/Amphorae/Forbidden")]
        [InlineData("/Amphorae/Index")]
        [InlineData("/Amphorae/Invite")]
        public async Task CanLoadPage_AsOwner(string path)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage_AsOwner));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());
            Assert.NotNull(dto?.Id);
            var id = dto.Id;
            var response = await adminClient.GetAsync($"{path}?id={id}");
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Description")]
        [InlineData("Files")]
        [InlineData("Signals")]
        [InlineData("TermsOfUse")]
        [InlineData("Location")]
        [InlineData("Issues")]
        [InlineData("Quality")]
        [InlineData("Options")]
        [InlineData("Access")]
        public async Task CanLoadPage_DetailsPages_AsOwner(string page)
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanLoadPage_AsOwner));
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());
            Assert.NotNull(dto?.Id);
            var id = dto.Id;
            var response = await adminClient.GetAsync($"/Amphorae/Detail/{page}?id={id}");
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CanLoadImage()
        {
            var path = "/Amphorae/StaticMap";
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());
            var id = dto.Id;
            await Task.Delay(500); // wait 0.5 seconds
            var response = await adminClient.GetAsync($"{path}?id={id}");
            await AssertHttpSuccess(response);
            Assert.Contains("image/", response.Content.Headers.ContentType.ToString());

            await DestroyAmphoraAsync(adminClient, id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}