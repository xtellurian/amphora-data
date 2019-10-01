using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Users;
using Amphora.Common.Models.Organisations;
using Newtonsoft.Json;

namespace Amphora.Tests.Helpers
{
    public static class AuthHelpers
    {

        public static void AddCreateToken(this HttpClient client)
        {
            if (!client.DefaultRequestHeaders.TryGetValues("Create", out var x))
            {
                client.DefaultRequestHeaders.Add("Create", "dev");
            }
        }
        public static async Task<(UserDto User, string Password)> CreateUserAsync(
            this HttpClient client,
            string fullName)
        {
            client.AddCreateToken();
            // first, create an organisation

            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";

            var user = new UserDto
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/users", content);
            var password = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            await client.GetTokenAsync(user, password);

            return (User: user, Password: password);
        }

        public static async Task<OrganisationDto> CreateOrganisationAsync(this HttpClient client, string testName)
        {
            var a = Helpers.EntityLibrary.GetOrganisationDto(testName);
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/organisations", requestBody);
            var createResponseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var org = JsonConvert.DeserializeObject<OrganisationDto>(createResponseContent);
            return org;
        }

        public static async Task GetTokenAsync(this HttpClient client, UserDto user, string password)
        {
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
        }
    }
}