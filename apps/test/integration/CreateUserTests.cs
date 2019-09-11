using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class CreateUserTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public CreateUserTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }


        [Fact]
        public async Task CanCreateUserWithCreateHeader()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Create", "dev");

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

            // now delete
            var deleteRequest = await client.DeleteAsync($"api/users/{user.UserName}");
            deleteRequest.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CanCreate_FirstOrgansation_AsNewUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Create", "dev");

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

            var org = new OrganisationModel()
            {
                Name = nameof(CreateUserTests),
            };
            
            response = await client.PostAsJsonAsync("api/organisations", org);
            var orgCreateContent = await response.Content.ReadAsStringAsync();
            var createdOrg = JsonConvert.DeserializeObject<OrganisationModel>(orgCreateContent);

            // now delete the org
            await client.DeleteAsync($"api/organisations/{createdOrg.OrganisationId}");
            // now delete user
            var deleteRequest = await client.DeleteAsync($"api/users/{user.UserName}");
            deleteRequest.EnsureSuccessStatusCode();
        }
    }
}