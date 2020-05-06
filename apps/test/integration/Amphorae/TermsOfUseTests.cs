using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class TermsOfUseTests : WebAppIntegrationTestBase
    {
        public TermsOfUseTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Creator_CanCreateThenDelete()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var tou = new TermsOfUse
            {
                Name = System.Guid.NewGuid().ToString(),
                Contents = System.Guid.NewGuid().ToString()
            };

            var result = await adminClient.PostAsJsonAsync($"api/TermsOfUse", tou);
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var dto = JsonConvert.DeserializeObject<TermsOfUse>(contents);
            Assert.NotNull(dto.Id);
            Assert.Equal(tou.Name, dto.Name);
            Assert.Equal(tou.Contents, dto.Contents);

            var response = await adminClient.GetAsync("api/TermsOfUse");
            await AssertHttpSuccess(response);
            var allTnc = JsonConvert.DeserializeObject<List<TermsOfUse>>(await response.Content.ReadAsStringAsync());
            Assert.NotEmpty(allTnc);
            Assert.Contains(allTnc, _ => _.Id == dto.Id);

            // get specific one
            var getRes = await adminClient.GetAsync($"api/TermsOfUse/{dto.Id}");
            await AssertHttpSuccess(getRes);
            var getTou = JsonConvert.DeserializeObject<TermsOfUse>(await getRes.Content.ReadAsStringAsync());
            Assert.Equal(dto.Id, getTou.Id);
            Assert.Equal(dto.Name, getTou.Name);
            Assert.Equal(dto.Contents, getTou.Contents);

            // now delete
            var deleteRes = await adminClient.DeleteAsync($"api/TermsOfUse/{dto.Id}");
            await AssertHttpSuccess(deleteRes);

            // now try get, and it's not there
            var getAgainRes = await adminClient.GetAsync($"api/TermsOfUse/{dto.Id}");
            Assert.False(getAgainRes.IsSuccessStatusCode);
        }

        [Fact]
        public async Task OtherUser_CantDeleteTermsOfUse()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var tou = new TermsOfUse
            {
                Name = System.Guid.NewGuid().ToString(),
                Contents = System.Guid.NewGuid().ToString()
            };

            var result = await adminClient.PostAsJsonAsync($"api/TermsOfUse", tou);
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var dto = JsonConvert.DeserializeObject<TermsOfUse>(contents);
            Assert.NotNull(dto.Id);

            // now delete
            var deleteRes = await otherClient.DeleteAsync($"api/TermsOfUse/{dto.Id}");
            Assert.False(deleteRes.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, deleteRes.StatusCode);
        }

        [Fact]
        public async Task Creator_CantDelete_IfAmphoraReferToTou()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var tou = new TermsOfUse
            {
                Name = System.Guid.NewGuid().ToString(),
                Contents = System.Guid.NewGuid().ToString()
            };

            var result = await adminClient.PostAsJsonAsync($"api/TermsOfUse", tou);
            await AssertHttpSuccess(result);
            tou = JsonConvert.DeserializeObject<TermsOfUse>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(tou.Id);

            // create the amphora
            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            amphora.TermsOfUseId = tou.Id;
            var createRes = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            await AssertHttpSuccess(createRes);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await createRes.Content.ReadAsStringAsync());

            // now try delete
            var deleteRes = await adminClient.DeleteAsync($"api/TermsOfUse/{tou.Id}");
            Assert.False(deleteRes.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, deleteRes.StatusCode);
            Assert.NotEmpty(await deleteRes.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GlobalTerms_CanBeReferencedByAnothersAmphora()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync("example.com");

            var tou = new TermsOfUse
            {
                Name = System.Guid.NewGuid().ToString(),
                Contents = System.Guid.NewGuid().ToString()
            };

            var result = await adminClient.PostAsJsonAsync($"api/GlobalTermsOfUse", tou);
            await AssertHttpSuccess(result);
            tou = JsonConvert.DeserializeObject<TermsOfUse>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(tou.Id);

            // other lists available terms, and it's there
            var listRes = await otherClient.GetAsync("api/TermsOfUse");
            await AssertHttpSuccess(listRes);
            var allTou = JsonConvert.DeserializeObject<List<TermsOfUse>>(await listRes.Content.ReadAsStringAsync());
            Assert.NotNull(allTou);
            Assert.NotEmpty(allTou);
            Assert.Contains(allTou, _ => _.Id == tou.Id);

            // create the amphora
            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            amphora.TermsOfUseId = tou.Id;
            var createRes = await otherClient.PostAsJsonAsync("api/amphorae", amphora);
            await AssertHttpSuccess(createRes);
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(await createRes.Content.ReadAsStringAsync());

            // now delete
            var deleteRes = await otherClient.DeleteAsync($"api/amphorae/{amphora.Id}");
            await AssertHttpSuccess(deleteRes);
        }

        [Fact]
        public async Task NormalUser_CantCreateGlobal()
        {
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync("example.com");

            var tou = new TermsOfUse
            {
                Name = System.Guid.NewGuid().ToString(),
                Contents = System.Guid.NewGuid().ToString()
            };

            var result = await otherClient.PostAsJsonAsync($"api/GlobalTermsOfUse", tou);
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}