using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Organisations
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class OrganisationCRUDTests : IntegrationTestBase
    {
        private int _apiVersion = 0;
        public OrganisationCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanCreateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (user, _) = await client.CreateUserAsync(email, nameof(CanCreateOrganisation));

            var a = Helpers.EntityLibrary.GetOrganisationDto(nameof(CanCreateOrganisation));

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
            var b = JsonConvert.DeserializeObject<OrganisationDto>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Name, b.Name);

            await DeleteOrganisation(b, client);
            await DestroyUserAsync(client, user);
        }

        [Theory]
        [InlineData("/api/organisations")]
        public async Task CanUpdateOrganisation(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (user, _) = await client.CreateUserAsync(email, nameof(CanUpdateOrganisation));
            var a = Helpers.EntityLibrary.GetOrganisationDto(nameof(CanUpdateOrganisation));
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PostAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            a = JsonConvert.DeserializeObject<OrganisationDto>(responseBody);

            // Act
            a.Name = System.Guid.NewGuid().ToString();
            requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync(url + "/" + a.Id, requestBody);
            updateResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<OrganisationModel>(await updateResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.Name, b.Name);

            await DeleteOrganisation(a, client);
            await DestroyUserAsync(client, user);
        }

        [Fact]
        public async Task CanInviteToOrganisation()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (user, _) = await client.CreateUserAsync(email, nameof(CanInviteToOrganisation));
            var org = Helpers.EntityLibrary.GetOrganisationDto(nameof(CanInviteToOrganisation));
            var requestBody = new StringContent(JsonConvert.SerializeObject(org), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PostAsync("api/organisations", requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            org = JsonConvert.DeserializeObject<OrganisationDto>(responseBody);

            var client2 = _factory.CreateClient();
            client2.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var email2 = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (otherUser, _) = await client2.CreateUserAsync(email2, nameof(CanInviteToOrganisation));

            var inviteResponse = await client.PostAsJsonAsync($"api/invitations/",
                new InvitationDto { TargetEmail = otherUser.Email, TargetOrganisationId = org.Id });
            var inviteResponseContent = await inviteResponse.Content.ReadAsStringAsync();
            inviteResponse.EnsureSuccessStatusCode();

            var acceptDto = new AcceptInvitationDto { TargetOrganisationId = org.Id };
            var acceptResponse = await client2.PostAsJsonAsync($"api/invitations/{org.Id}", acceptDto);
            acceptResponse.EnsureSuccessStatusCode();

            var selfResponse = await client2.GetAsync("api/users/self");
            var selfContent = await selfResponse.Content.ReadAsStringAsync();
            selfResponse.EnsureSuccessStatusCode();
            var self = JsonConvert.DeserializeObject<UserDto>(selfContent);
            Assert.Equal(org.Id, self.OrganisationId);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
            await DestroyUserAsync(client2, otherUser);
        }

        private async Task DeleteOrganisation(OrganisationDto a, HttpClient client)
        {
            var deleteResponse = await client.DeleteAsync($"api/organisations/{a.Id}");
            deleteResponse.EnsureSuccessStatusCode();
            var getResponse = await client.GetAsync($"api/organisations/{a.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}