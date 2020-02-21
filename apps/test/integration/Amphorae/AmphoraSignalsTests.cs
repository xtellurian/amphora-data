using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class AmphoraSignalsTests : IntegrationTestBase
    {
        private const string BadName = "HH78365^@*";
        private const string NameWithSpace = "hello world";
        public AmphoraSignalsTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateSignalOnAmphora()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var amphora = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", amphora);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // create a signal
            var generator = new RandomGenerator(1);
            var property = generator.RandomString(1).ToLower() + generator.RandomString(10) + "_" + generator.RandomString(2); // w/ underscore
            var signalDto = EntityLibrary.GetSignalDto(property);
            var response = await adminClient.PostAsJsonAsync($"api/amphorae/{amphora.Id}/signals", signalDto);
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var createdSignal = JsonConvert.DeserializeObject<Signal>(responseContent);
            Assert.NotNull(createdSignal);
            Assert.NotNull(createdSignal.Id);
            Assert.Equal(signalDto.Attributes, createdSignal.Attributes);
            Assert.Equal(signalDto.Property, createdSignal.Property);
            Assert.Equal(signalDto.ValueType, createdSignal.ValueType);

            // check we can get the signal from the API
            var signalsResponse = await adminClient.GetAsync($"api/amphorae/{amphora.Id}/signals");
            var listContent = await signalsResponse.Content.ReadAsStringAsync();
            signalsResponse.EnsureSuccessStatusCode();
            var signals = JsonConvert.DeserializeObject<List<Signal>>(listContent);
            Assert.NotNull(signals);
            Assert.NotEmpty(signals);
            Assert.Contains(signals, s => s.Id == createdSignal.Id);
        }

        [Fact]
        public async Task CreateSignalOnAmphora_Symbol_Error()
        {
            var testName = nameof(CanCreateSignalOnAmphora);
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var dto = EntityLibrary.GetAmphoraDto(adminOrg.Id, testName);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // create a signal
            var generator = new RandomGenerator(1);
            var signalDto = EntityLibrary.GetSignalDto(BadName);
            var response = await adminClient.PostAsJsonAsync($"api/amphorae/{dto.Id}/signals", signalDto);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateSignalOnAmphora_Space_Error()
        {
            var testName = nameof(CanCreateSignalOnAmphora);
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var dto = EntityLibrary.GetAmphoraDto(adminOrg.Id, testName);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // create a signal
            var generator = new RandomGenerator(1);
            var signalDto = EntityLibrary.GetSignalDto(NameWithSpace);
            var response = await adminClient.PostAsJsonAsync($"api/amphorae/{dto.Id}/signals", signalDto);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CanUpdateSignal()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            // create an amphora
            var dto = EntityLibrary.GetAmphoraDto(adminOrg.Id);
            var createResponse = await adminClient.PostAsJsonAsync("api/amphorae", dto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(createContent);

            // create a signal
            var generator = new RandomGenerator(1);
            var property = generator.RandomString(10) + "_" + generator.RandomString(2); // w/ underscore
            var signal = EntityLibrary.GetSignalDto(property.ToLower());
            var response = await adminClient.PostAsJsonAsync($"api/amphorae/{dto.Id}/signals", signal);
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            signal = JsonConvert.DeserializeObject<Signal>(responseContent);

            // set some metadata on the updated signal
            var updatedMetadata = new Dictionary<string, string>()
            {
                { System.Guid.NewGuid().ToString(), System.Guid.NewGuid().ToString() },
                { System.Guid.NewGuid().ToString(), System.Guid.NewGuid().ToString() },
                { System.Guid.NewGuid().ToString(), System.Guid.NewGuid().ToString() }
            };

            var updateRes = await adminClient.PutAsJsonAsync($"api/amphorae/{dto.Id}/signals/{signal.Id}", new UpdateSignal(updatedMetadata));
            Assert.True(updateRes.IsSuccessStatusCode);

            var content = await updateRes.Content.ReadAsStringAsync();
            signal = JsonConvert.DeserializeObject<Signal>(content);
            Assert.NotNull(signal.Attributes);
            Assert.Equal(updatedMetadata, signal.Attributes);
        }
    }
}
