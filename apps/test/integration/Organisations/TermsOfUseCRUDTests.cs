using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Organisations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class TermsOfUseCRUDTests : WebAppIntegrationTestBase
    {
        public TermsOfUseCRUDTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateTerms()
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

            var response = await adminClient.GetAsync($"api/TermsOfUse");
            await AssertHttpSuccess(response);
            var allTnc = JsonConvert.DeserializeObject<List<TermsOfUse>>(await response.Content.ReadAsStringAsync());
            Assert.Single(allTnc);
            Assert.Equal(dto.Id, allTnc[0].Id);
            Assert.Equal(dto.Contents, allTnc[0].Contents);
        }
    }
}