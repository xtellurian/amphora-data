using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Tests.Helpers;
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
        public async Task Post_CreatesAmphora(string url)
        {
            // Arrange
            var (client, user, org) = await GetAuthenticatedClientAsync();
            var a = Helpers.EntityLibrary.GetAmphora(org.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync(url, requestBody);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Title, b.Title);

            await DeleteAmphora(client, b.Id);
            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ListsAmphorae(string url)
        {
            // Arrange
            var (client, user, org) = await GetAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(org.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            var amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(await createResponse.Content.ReadAsStringAsync());
            createResponse.EnsureSuccessStatusCode();
            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());


            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Amphora>>(responseBody);
            Assert.True(b.Count > 0);

            await DeleteAmphora(client, amphora.Id);
            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ReadsAmphora(string url)
        {
            // Arrange
            var (client, user, org) = await GetAuthenticatedClientAsync();

            var a = Helpers.EntityLibrary.GetAmphora(org.OrganisationId);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(await createResponse.Content.ReadAsStringAsync());

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync($"{url}/{b.Id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());


            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Title, c.Title);

            // cleanup
            await DeleteAmphora(client, b.Id);
            await DestroyUserAsync(client);
            await DestroyOrganisationAsync(client);
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