using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class CreateUserTests : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public CreateUserTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
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
                Email = email
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
    }
}