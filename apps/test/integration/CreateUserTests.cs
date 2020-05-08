using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class CreateUserTests : WebAppIntegrationTestBase
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
            var password = System.Guid.NewGuid().ToString() + "!A";
            // Act
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var user = new CreateAmphoraUser
            {
                FullName = "test name",
                UserName = email,
                Email = email,
                Password = password,
                ConfirmPassword = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/users", content);

            // Assert
            await AssertHttpSuccess(response);
            // can log in
            var loginRequest = new LoginRequest()
            {
                Password = password,
                Username = user.UserName
            };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            await AssertHttpSuccess(loginResponse);

            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task CanCreate_FirstOrgansation_AsNewUser()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, _apiVersion.ToString());
            var password = System.Guid.NewGuid().ToString() + "1A!";
            // Act
            var email = System.Guid.NewGuid().ToString() + "@example.com";
            var user = new CreateAmphoraUser
            {
                FullName = nameof(CanCreate_FirstOrgansation_AsNewUser),
                UserName = email,
                Email = email,
                Password = password,
                ConfirmPassword = password
            };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            // flaky
            var response = await client.PostAsync("api/users", content);

            if (!response.IsSuccessStatusCode)
            {
                // wait 0.5 seconds and retry due to flakiness
                response = await client.PostAsync("api/users", content);
            }

            await AssertHttpSuccess(response);

            var loginRequest = new LoginRequest()
            {
                Password = password,
                Username = user.UserName
            };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            await AssertHttpSuccess(loginResponse);
            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var org = EntityLibrary.GetOrganisationDto();

            response = await client.PostAsJsonAsync("api/organisations", org);
            var orgCreateContent = await response.Content.ReadAsStringAsync();
            await AssertHttpSuccess(response);
            var createdOrg = JsonConvert.DeserializeObject<Organisation>(orgCreateContent);

            // now delete the org
            await client.DeleteAsync($"api/organisations/{createdOrg.Id}");
        }

        [Trait("Phase", "One")]
        [Theory]
        [InlineData(Users.AmphoraAdmin)]
        [InlineData(Users.Attacker)]
        [InlineData(Users.Standard, Users.StandardTwo)]
        [InlineData(Users.Other)]
        public async Task TestPersonas_EnsureIsCreated(string persona, string second = null)
        {
            await CheckPersonaAsync(await GetPersonaAsync(persona));
            if (second != null)
            {
                await CheckPersonaAsync(await GetPersonaAsync(second));
            }
        }

        private static async Task CheckPersonaAsync(Persona admin)
        {
            Assert.NotNull(admin.Organisation?.Id);
            Assert.NotNull(admin.User?.Id);
            var getSelfRes = await admin.Http.GetAsync("api/users/self");
            getSelfRes.EnsureSuccessStatusCode();
        }
    }
}