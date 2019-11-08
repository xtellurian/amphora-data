using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class SignalDataTests: IntegrationTestBase
    {
        public SignalDataTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
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
            Assert.Equal(System.Net.HttpStatusCode.NotFound , response.StatusCode);
        }

        [Fact]
        public async Task CanUploadSignal()
        {
        // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(CanUploadSignal));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync("api/amphorae", requestBody);
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            a = JsonConvert.DeserializeObject<AmphoraExtendedDto>(responseBody);
            Assert.NotNull(a.Id);
            // create a signal
            var dSignal = new SignalDto(){Property="d", ValueType= "Numeric"};
            var sSignal = new SignalDto(){Property="s", ValueType= "String"};

            var dRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", dSignal);
            var sRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals", sSignal);

            dRes.EnsureSuccessStatusCode();
            sRes.EnsureSuccessStatusCode();

            // act
            var values = new SignalValuesDto()
            {
                SignalValues = new List<PropertyValuePair>
                {
                    new PropertyValuePair("d", 5),
                    new PropertyValuePair("s", "hello")
                }
            };
            adminClient.DefaultRequestHeaders.Add("X-Api-Version", "Nov-19");
            var valRes = await adminClient.PostAsJsonAsync($"api/amphorae/{a.Id}/signals/values", values);
            valRes.EnsureSuccessStatusCode();


            await DestroyAmphoraAsync(adminClient, a.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }
    }
}