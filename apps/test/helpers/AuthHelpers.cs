using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Helpers
{
    public static class AuthHelpers
    {
        public static async Task<(AmphoraUser User, string Password)> CreateUserAsync(
            this HttpClient client,
            string email,
            string fullName)
        {
            // assumed the user has been invited
            var user = new AmphoraUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
            };
            var requestPath = "api/users";
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestPath, content);
            var password = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, "Content: " + await response.Content.ReadAsStringAsync());
            await client.GetTokenAsync(user, password);

            return (User: user, Password: password);
        }

        public static async Task<Organisation> CreateOrganisationAsync(this HttpClient client, string testName)
        {
            var a = Helpers.EntityLibrary.GetOrganisationDto(testName);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/organisations", requestBody);
            var createResponseContent = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, "Content: " + await response.Content.ReadAsStringAsync());
            var org = JsonConvert.DeserializeObject<Organisation>(createResponseContent);
            return org;
        }

        public static async Task GetTokenAsync(this HttpClient client, AmphoraUser user, string password)
        {
            // can log in
            var loginRequest = new TokenRequest()
            {
                Password = password,
                Username = user.UserName
            };

            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            Assert.True(loginResponse.IsSuccessStatusCode, "Content: " + await loginResponse.Content.ReadAsStringAsync());

            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}