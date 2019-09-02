using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class OrganisationCRUDTests : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public OrganisationCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanCreateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetOrganisation();
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
            var b = JsonConvert.DeserializeObject<Organisation>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Name, b.Name);
            Assert.Equal(a.InviteCode, b.InviteCode);

            await DeleteOrganisation(b, client);

        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanUpdateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetOrganisation();
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            a = JsonConvert.DeserializeObject<Organisation>(responseBody);

            // Act
            a.Name = Guid.NewGuid().ToString();
            requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync(url + "/" + a.OrganisationId, requestBody);
            updateResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<Organisation>(await updateResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.OrganisationId, b.OrganisationId);
            Assert.Equal(a.Name, b.Name);

            await DeleteOrganisation(a, client);

        }

        private async Task DeleteOrganisation(Organisation a, HttpClient client)
        {
            var deleteResponse = await client.DeleteAsync($"api/organisations/{a.OrganisationId}");
            deleteResponse.EnsureSuccessStatusCode();
            var getResponse = await client.GetAsync($"api/organisations/{a.OrganisationId}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}