using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraQualityTests : WebAppIntegrationTestBase
    {
        public AmphoraQualityTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Amphora_CanSetAndGetQuality_AsCreator()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsJsonAsync("/api/amphorae", amphora);
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var quality = new Quality(QualityLevels.Low, QualityLevels.Medium, QualityLevels.High, QualityLevels.Perfect);
            var setQualityRes = await adminClient.PostAsJsonAsync($"/api/amphorae/{amphora.Id}/Quality", quality);
            await AssertHttpSuccess(setQualityRes);

            var getQualityRes = await adminClient.GetAsync($"/api/amphorae/{amphora.Id}/Quality");
            await AssertHttpSuccess(getQualityRes);
            var getQ = JsonConvert.DeserializeObject<Quality>(await getQualityRes.Content.ReadAsStringAsync());
            Assert.NotNull(getQ);
            Assert.Equal(quality.Accuracy, getQ.Accuracy);
            Assert.Equal(quality.Completeness, getQ.Completeness);
            Assert.Equal(quality.Granularity, getQ.Granularity);
            Assert.Equal(quality.Reliability, getQ.Reliability);
        }

        [Fact]
        public async Task Amphora_NoQuality_ReturnsQualityWithNulls()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsJsonAsync("/api/amphorae", amphora);
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var getQualityRes = await adminClient.GetAsync($"/api/amphorae/{amphora.Id}/Quality");
            await AssertHttpSuccess(getQualityRes);
            var getQ = JsonConvert.DeserializeObject<Quality>(await getQualityRes.Content.ReadAsStringAsync());
            Assert.NotNull(getQ);
            Assert.Null(getQ.Accuracy);
            Assert.Null(getQ.Completeness);
            Assert.Null(getQ.Granularity);
            Assert.Null(getQ.Reliability);
        }

        [Fact]
        public async Task Amphora_SettingQualityOutOfRange_ReturnsError()
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id);
            // create an amphora for us to work with
            var createResponse = await adminClient.PostAsJsonAsync("/api/amphorae", amphora);
            await AssertHttpSuccess(createResponse);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            amphora = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            var quality = new { Accuracy = 0, Completeness = "hello" };
            var setQualityRes = await adminClient.PostAsJsonAsync($"/api/amphorae/{amphora.Id}/Quality", quality);
            Assert.False(setQualityRes.IsSuccessStatusCode);
            Assert.NotEmpty(await setQualityRes.Content.ReadAsStringAsync());
        }
    }
}
