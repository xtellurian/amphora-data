using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Amphorae
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AmphoraCRUDTests : WebAppIntegrationTestBase
    {
        public AmphoraCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory) { }

        [Fact]
        public async Task Post_CreatesAmphora_AsAdmin()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Name, b.Name);
            Assert.Equal(a.Labels, b.Labels);

            // check it's in the list
            var listRes = await adminClient.GetAsync("/api/amphorae?accessType=created&scope=self");
            await AssertHttpSuccess(listRes);
            var listContent = await listRes.Content.ReadAsStringAsync();
            var selfCreatedAmphora = JsonConvert.DeserializeObject<List<DetailedAmphora>>(listContent);
            Assert.NotNull(selfCreatedAmphora);
            Assert.NotEmpty(selfCreatedAmphora);
            Assert.Single(selfCreatedAmphora);

            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task Get_ReadsAmphora_AsAdmin()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            await AssertHttpSuccess(createResponse);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.GetAsync($"{url}/{b.Id}");

            // Assert
            await AssertHttpSuccess(response);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Name, c.Name);

            // cleanup
            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task Get_PublicAmphora_AllowAccessToAny()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_PublicAmphora_AllowAccessToAny));
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            var c = await createResponse.Content.ReadAsStringAsync();
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(c);
            await AssertHttpSuccess(createResponse);
            // Act
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var response = await client.GetAsync($"{url}/{dto.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            await AssertHttpSuccess(response);
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(responseContent);
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
            var (otherClient, otherUser, otherOrg) = await NewOrgAuthenticatedClientAsync();

            var dto = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Get_ReadsAmphora_AsAdmin));
            var requestBody = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync("api/amphorae", requestBody);
            await AssertHttpSuccess(createResponse);
            dto = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());

            var purchaseResponse = await otherClient.PostAsync($"api/Amphorae/{dto.Id}/Purchases", null);
            await AssertHttpSuccess(purchaseResponse);

            var readResponse = await adminClient.GetAsync($"api/amphorae/{dto.Id}");
            var content = await readResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(readResponse);

            var b = JsonConvert.DeserializeObject<DetailedAmphora>(content);
            Assert.Equal(dto.Description, b.Description);
            Assert.True(b.Lat.HasValue);
            Assert.True(b.Lon.HasValue);
            Assert.Equal(dto.Lat, b.Lat);
            Assert.Equal(dto.Lon, b.Lon);

            await DestroyAmphoraAsync(adminClient, dto.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task CreateAmphora_MissingTermsAndConditions_ShouldError()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            a.TermsAndConditionsId = System.Guid.NewGuid().ToString();
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            Assert.False(response.IsSuccessStatusCode);

            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task UpdateAmphora_Success()
        {
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(responseBody);
            Assert.NotNull(a.Id);

            // now update
            var newName = System.Guid.NewGuid().ToString();
            var newDescription = System.Guid.NewGuid().ToString();
            var newLabels = System.Guid.NewGuid().ToString();

            a.Name = newName;
            a.Description = newDescription;
            a.Labels = newLabels;
            var updateResponse = await adminClient.PutAsJsonAsync($"{url}/{a.Id}", a);
            var updateResponseBody = await updateResponse.Content.ReadAsStringAsync();
            var b = JsonConvert.DeserializeObject<DetailedAmphora>(updateResponseBody);

            // Assert
            await AssertHttpSuccess(updateResponse);
            Assert.NotNull(b);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.Price, b.Price);
            // the updated properties
            Assert.Equal(newDescription, b.Description);
            Assert.Equal(newName, b.Name);
            Assert.Equal(newLabels, b.Labels);

            await DestroyAmphoraAsync(adminClient, b.Id);
            await DestroyOrganisationAsync(adminClient, adminOrg);
            await DestroyUserAsync(adminClient, adminUser);
        }

        [Fact]
        public async Task AmphoraWithLongName_CannotBeCreated()
        {
            var gen = new Helpers.RandomGenerator();
            var url = "/api/amphorae";
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphoraDto(adminOrg.Id, nameof(Post_CreatesAmphora_AsAdmin));
            a.Name = gen.RandomString(200);
            var s = JsonConvert.SerializeObject(a);
            var requestBody = new StringContent(s, Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            Assert.NotEmpty(responseBody);
        }
    }
}