using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Organisations;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Organisations
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class TermsAndConditionsCRUDTests : IntegrationTestBase
    {
        public TermsAndConditionsCRUDTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateTermsAndConditions()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var tnc = new TermsAndConditions();
            tnc.Id = System.Guid.NewGuid().ToString();
            tnc.Name = System.Guid.NewGuid().ToString();
            tnc.Contents = System.Guid.NewGuid().ToString();

            var result = await adminClient.PostAsJsonAsync($"api/Organisations/{adminOrg.Id}/TermsAndConditions", tnc);
            var contents = await result.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();

            var dto = JsonConvert.DeserializeObject<TermsAndConditions>(contents);
            Assert.Equal(tnc.Id, dto.Id);
            Assert.Equal(tnc.Contents, dto.Contents);

            var response = await adminClient.GetAsync($"api/Organisations/{adminOrg.Id}/TermsAndConditions");
            var contents2 = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var allTnc = JsonConvert.DeserializeObject<List<TermsAndConditions>>(contents2);
            Assert.Single(allTnc);
            Assert.Equal(dto.Id, allTnc[0].Id);
            Assert.Equal(dto.Contents, allTnc[0].Contents);
        }
    }
}