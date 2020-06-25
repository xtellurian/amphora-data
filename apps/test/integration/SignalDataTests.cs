using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Amphorae.Signals;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class SignalDataTests : WebAppIntegrationTestBase
    {
        public SignalDataTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task UploadSignalValuesTo_MissingAmphora(string url)
        {
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            var id = System.Guid.NewGuid();
            var json = Helpers.BadJsonLibrary.GetJson(Helpers.BadJsonLibrary.DiverseTypesKey);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync($"{url}/{id}/signals/values", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CaseMismatch_Error()
        {
            // Arrange
            var p1 = "der";
            var p2 = "sed";

            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanUploadSignal));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync("api/amphorae", requestBody);
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(a.Id);
            // create a signal
            var dSignal = EntityLibrary.GetSignalDto(p1, "Numeric");
            var sSignal = EntityLibrary.GetSignalDto(p2, "String");

            var dRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", dSignal);
            var sRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", sSignal);

            await AssertHttpSuccess(dRes);
            await AssertHttpSuccess(sRes);

            // act
            var vals = new Dictionary<string, object>
                {
                    { p1.ToUpper(), 5 }, // uppercase should error
                    { p2.ToUpper(), "hello" }
                };

            var valRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals/values", vals);
            Assert.False(valRes.IsSuccessStatusCode);

            var batchVals = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { p1, 6 },
                    { p2, "batch1" }
                },
                new Dictionary<string, object>
                {
                    { p1.ToUpper(), 7 },
                    { p2.ToUpper(), "batch2" }
                }
            };

            var batchRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals/batchvalues", batchVals);
            Assert.False(batchRes.IsSuccessStatusCode);

            await DestroyAmphoraAsync(adminClient, a.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CanUploadSignal()
        {
            // Arrange
            var p1 = "der";
            var p2 = "sed";

            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanUploadSignal));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync("api/amphorae", requestBody);
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(a.Id);
            // create a signal
            var dSignal = new Signal() { Property = p1, ValueType = "Numeric" };
            var sSignal = new Signal() { Property = p2, ValueType = "String" };

            var dRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", dSignal);
            var sRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", sSignal);
            var content = await dRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(dRes);
            await AssertHttpSuccess(sRes);

            // act

            var vals = new Dictionary<string, object>
                {
                    { p1, 5 },
                    { p2, "hello" }
                };

            var valRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals/values", vals);
            await AssertHttpSuccess(valRes);

            var batchVals = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { p1, 6 },
                    { p2, "batch1" }
                },
                new Dictionary<string, object>
                {
                    { p1, 7 },
                    { p2, "batch2" }
                }
            };

            var batchRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals/batchvalues", batchVals);
            await AssertHttpSuccess(batchRes);

            await DestroyAmphoraAsync(adminClient, a.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}