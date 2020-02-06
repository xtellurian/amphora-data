using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class CreateUserTests : IntegrationTestBase
    {
        private int _apiVersion = 0;
        public CreateUserTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateUser_FromAmphoraDataDomain()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());

            // Act
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var user = new AmphoraUser
            {
                UserName = email,
                Email = email,
            };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/users", content);
            var password = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            // can log in
            var loginRequest = new TokenRequest()
            {
                Password = password,
                Username = user.UserName
            };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            loginResponse.EnsureSuccessStatusCode();

            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            // now delete
            var deleteRequest = await client.DeleteAsync($"api/users/{user.UserName}");
            deleteRequest.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CanCreate_FirstOrgansation_AsNewUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());

            // Act
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
            };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/users", content);
            var password = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var loginRequest = new TokenRequest()
            {
                Password = password,
                Username = user.UserName
            };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            loginResponse.EnsureSuccessStatusCode();
            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var org = EntityLibrary.GetOrganisationDto();

            response = await client.PostAsJsonAsync("api/organisations", org);
            var orgCreateContent = await response.Content.ReadAsStringAsync();
            var createdOrg = JsonConvert.DeserializeObject<Organisation>(orgCreateContent);

            // now delete the org
            await client.DeleteAsync($"api/organisations/{createdOrg.Id}");
            // now delete user
            var deleteRequest = await client.DeleteAsync($"api/users/{user.UserName}");
            deleteRequest.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task EmailCantSignupWithoutInvitation()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());

            // Act
            var email = System.Guid.NewGuid().ToString() + "@example.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
            };

            var response = await client.PostAsJsonAsync("api/users", user);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}