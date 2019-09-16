using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class OrganisationCRUDTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public OrganisationCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {
        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanCreateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.AddCreateToken();
            var (user, _) = await client.CreateUserAsync(nameof(CanCreateOrganisation));

            var a = Helpers.EntityLibrary.GetOrganisation(nameof(CanCreateOrganisation));

            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync(url, requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<OrganisationModel>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Name, b.Name);

            await DeleteOrganisation(b, client);
            await DestroyUserAsync(client);

        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanUpdateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.AddCreateToken();
            var (user, _) = await client.CreateUserAsync(nameof(CanUpdateOrganisation));
            var a = Helpers.EntityLibrary.GetOrganisation(nameof(CanUpdateOrganisation));
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            a = JsonConvert.DeserializeObject<OrganisationModel>(responseBody);

            // Act
            a.Name = Guid.NewGuid().ToString();
            requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync(url + "/" + a.OrganisationId, requestBody);
            updateResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<OrganisationModel>(await updateResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.OrganisationId, b.OrganisationId);
            Assert.Equal(a.Name, b.Name);

            await DeleteOrganisation(a, client);
            await DestroyUserAsync(client);

        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanInviteToOrganisation(string url)
        {
             // Arrange
            var client = _factory.CreateClient();
            client.AddCreateToken();
            var (user, _) = await client.CreateUserAsync(nameof(CanInviteToOrganisation));
            var org = Helpers.EntityLibrary.GetOrganisation(nameof(CanInviteToOrganisation));
            var requestBody = new StringContent(JsonConvert.SerializeObject(org), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync(url, requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            org = JsonConvert.DeserializeObject<OrganisationModel>(responseBody);


            var client2 = _factory.CreateClient();
            var (otherUser, _) = await client2.CreateUserAsync(nameof(CanInviteToOrganisation));

            var inviteResponse = await client.PostAsJsonAsync($"{url}/{org.OrganisationId}/invitations/",
                new Invitation(otherUser.Email));
            var inviteResponseContent = await inviteResponse.Content.ReadAsStringAsync();
            inviteResponse.EnsureSuccessStatusCode();

            var acceptResponse = await client2.GetAsync($"api/organisations/{org.OrganisationId}/invitations");
            acceptResponse.EnsureSuccessStatusCode();

            var selfResponse = await client2.GetAsync("api/users/self");
            var selfContent = await selfResponse.Content.ReadAsStringAsync();
            selfResponse.EnsureSuccessStatusCode();
            var self = JsonConvert.DeserializeObject<ApplicationUser>(selfContent);
            Assert.Equal(org.OrganisationId, self.OrganisationId);

            await DestroyOrganisationAsync(client);
            await DestroyUserAsync(client);
            await DestroyUserAsync(client2);
        }

        private async Task DeleteOrganisation(OrganisationModel a, HttpClient client)
        {
            var deleteResponse = await client.DeleteAsync($"api/organisations/{a.OrganisationId}");
            deleteResponse.EnsureSuccessStatusCode();
            var getResponse = await client.GetAsync($"api/organisations/{a.OrganisationId}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}