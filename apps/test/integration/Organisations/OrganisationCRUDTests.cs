using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Organisations
{
    [Collection(nameof(ApiFixtureCollection))]
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
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<Organisation>(responseBody);
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
            await AssertHttpSuccess(createResponse);
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            a = JsonConvert.DeserializeObject<Organisation>(responseBody);

            // Act
            a.Name = System.Guid.NewGuid().ToString();
            requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync(url + "/" + a.Id, requestBody);
            await AssertHttpSuccess(updateResponse);
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
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            Assert.NotNull(responseBody);
            org = JsonConvert.DeserializeObject<Organisation>(responseBody);

            var client2 = _factory.CreateClient();
            client2.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var email2 = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (otherUser, _) = await client2.CreateUserAsync(email2, nameof(CanInviteToOrganisation));

            var inviteResponse = await client.PostAsJsonAsync($"api/invitations/",
                new Invitation { TargetEmail = otherUser.Email, TargetOrganisationId = org.Id });
            var inviteResponseContent = await inviteResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(inviteResponse);

            var acceptDto = new AcceptInvitation { TargetOrganisationId = org.Id };
            var acceptResponse = await client2.PostAsJsonAsync($"api/invitations/{org.Id}", acceptDto);
            await AssertHttpSuccess(acceptResponse);

            var selfResponse = await client2.GetAsync("api/users/self");
            var selfContent = await selfResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(selfResponse);
            var self = JsonConvert.DeserializeObject<AmphoraUser>(selfContent);
            Assert.Equal(org.Id, self.OrganisationId);

            await DestroyOrganisationAsync(client, org);
            await DestroyUserAsync(client, user);
            await DestroyUserAsync(client2, otherUser);
        }

        private async Task DeleteOrganisation(Organisation a, HttpClient client)
        {
            var deleteResponse = await client.DeleteAsync($"api/organisations/{a.Id}");
            await AssertHttpSuccess(deleteResponse);
            var getResponse = await client.GetAsync($"api/organisations/{a.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}