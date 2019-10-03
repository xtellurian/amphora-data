using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraSignalsTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public AmphoraSignalsTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        [Fact]
        public async Task CanCreateSignalOnAmphora()
        {
            var testName = nameof(CanCreateSignalOnAmphora);
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
        
            // create an amphora
            var dto = EntityLibrary.GetAmphoraDto(adminOrg.Id, testName);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(createContent);

            // create a signal
            var keyName = System.Guid.NewGuid().ToString();
            var signalDto = EntityLibrary.GetSignalDto(keyName);
            var response = await adminClient.PostAsJsonAsync($"api/amphorae/{dto.Id}/signals", signalDto);
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            signalDto = JsonConvert.DeserializeObject<SignalDto>(responseContent);
            Assert.NotNull(signalDto);
            Assert.NotNull(signalDto.Id);

            // check we can get the signal from the API
            var signalsResponse = await adminClient.GetAsync($"api/amphorae/{dto.Id}/signals");
            var listContent = await signalsResponse.Content.ReadAsStringAsync();
            signalsResponse.EnsureSuccessStatusCode();
            var signals = JsonConvert.DeserializeObject<List<SignalDto>>(listContent);
            Assert.NotNull(signals);
            Assert.NotEmpty(signals);
            Assert.Contains(signals, s => s.Id == signalDto.Id);
        }
    }
}