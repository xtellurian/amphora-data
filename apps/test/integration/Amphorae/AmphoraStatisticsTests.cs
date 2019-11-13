using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class AmphoraStatisticsTests : IntegrationTestBase
    {
        public AmphoraStatisticsTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        [Fact]
        public async Task CanCreateSignalOnAmphora()
        {
            var testName = nameof(CanCreateSignalOnAmphora);
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            // Act
            var dto = EntityLibrary.GetAmphoraDto(adminOrg.Id, testName);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();

            // Assert
            var otherClient = _factory.CreateClient(); // get a non-auth'd client
            var res = await otherClient.GetAsync("api/amphoraeStats/count");
            var content = await res.Content.ReadAsStringAsync();
            res.EnsureSuccessStatusCode();
            var count = int.Parse(content);
            Assert.True(count >= 1); // there might be other tests running so count >> 1
        }
    }
}