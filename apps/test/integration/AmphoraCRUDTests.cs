using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraCRUDTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public AmphoraCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory) { }


        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_CreatesAmphora_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.PostAsync(url, requestBody);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.AmphoraModel>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Name, b.Name);

            await DeleteAmphora(adminClient, b.Id);
            await DestroyUserAsync(adminClient);
            await DestroyOrganisationAsync(adminClient);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ListsAmphorae_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            var amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.AmphoraModel>(await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());


            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<List<Amphora.Common.Models.AmphoraModel>>(responseBody);
            Assert.True(b.Count > 0);

            await DeleteAmphora(adminClient, amphora.Id);
            await DestroyUserAsync(adminClient);
            await DestroyOrganisationAsync(adminClient);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ReadsAmphora_AsAdmin(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.AmphoraModel>(await createResponse.Content.ReadAsStringAsync());

            // Act
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await adminClient.GetAsync($"{url}/{b.Id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());


            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<Amphora.Common.Models.AmphoraModel>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Name, c.Name);

            // cleanup
            await DeleteAmphora(adminClient, b.Id);
            await DestroyUserAsync(adminClient);
            await DestroyOrganisationAsync(adminClient);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_PublicAmphora_AllowAccessToAny(string url)
        {
            // Arrange
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(adminOrg.OrganisationId);
            a.IsPublic = true;
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            adminClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await adminClient.PostAsync(url, requestBody);
            a = JsonConvert.DeserializeObject<Amphora.Common.Models.AmphoraModel>(await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            // Act
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var response = await client.GetAsync($"{url}/{a.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert
            response.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<Common.Models.AmphoraModel>(responseContent);
            Assert.NotNull(b);
            Assert.Equal(a.Id, b.Id);
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            deleteResponse.EnsureSuccessStatusCode();
            var response = await client.GetAsync($"api/amphorae/{id}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}