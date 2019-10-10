using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class AmphoraCRUDTests : IntegrationTestBase
    {
        public AmphoraCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory) { }


        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_CreatesAmphora_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<AmphoraModel>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Name, b.Name);

            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ReadsAmphora_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.GetAsync($"{url}/{b.Id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());


            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<AmphoraExtendedDto>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Name, c.Name);

            // cleanup
            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_PublicAmphora_AllowAccessToAny(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_PublicAmphora_AllowAccessToAny));
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            var c = await createResponse.Content.ReadAsStringAsync();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(c);
            createResponse.EnsureSuccessStatusCode();
            // Act
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var response = await client.GetAsync($"{url}/{dto.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            response.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<AmphoraExtendedDto>(responseContent);
            Assert.NotNull(b);
            Assert.Equal(dto.Id, b.Id);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
            
            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);

        }

        [Fact]
        public async Task PurchaseAmphora_DetailsRemainSame()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            createResponse.EnsureSuccessStatusCode();
            dto = JsonConvert.DeserializeObject<AmphoraExtendedDto>(await createResponse.Content.ReadAsStringAsync());

            var purchaseResponse = await adminClient.PostAsync($"api/market/purchase?id={dto.Id}", null);
            purchaseResponse.EnsureSuccessStatusCode();

            var readResponse = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            var content = await readResponse.Content.ReadAsStringAsync();
            readResponse.EnsureSuccessStatusCode();

            var b = JsonConvert.DeserializeObject<AmphoraExtendedDto>(content);
            Assert.Equal(dto.Description, b.Description);
            Assert.True( b.Lat.HasValue);
            Assert.True( b.Lon.HasValue);
            Assert.Equal( dto.Lat, b.Lat);
            Assert.Equal( dto.Lon, b.Lon);
        }

    }
}