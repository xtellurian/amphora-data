using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Helpers
{
    public static class AuthHelpers
    {
        private const string PhoneNumber = "0412 345 678";
        public static async Task<(AmphoraUser User, string Password)> CreateUserAsync(
            this HttpClient client,
            string email,
            string fullName)
        {
            var password = System.Guid.NewGuid().ToString() + "!A1";
            // assumed the user has been invited
            var user = new CreateAmphoraUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Password = password,
                ConfirmPassword = password,
                PhoneNumber = PhoneNumber
            };
            var requestPath = "api/users";
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestPath, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, "Content: " + responseContent);
            var createdUser = JsonConvert.DeserializeObject<AmphoraUser>(responseContent);
            await client.GetTokenAsync(user.UserName, password);

            return (User: createdUser, Password: password);
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

        public static async Task SetPlan(this HttpClient client, string orgId, Common.Models.Organisations.Accounts.Plan.PlanTypes planType)
        {
            var setResult = await client.PostAsJsonAsync($"api/Organisations/{orgId}/Account/Plan?planType={planType}",
                    new object());
            setResult.EnsureSuccessStatusCode();
        }

        public static async Task GetTokenAsync(this HttpClient client, string userName, string password)
        {
            // can log in
            var loginRequest = new LoginRequest()
            {
                Password = password,
                Username = userName
            };

            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            Assert.True(loginResponse.IsSuccessStatusCode, "Content: " + await loginResponse.Content.ReadAsStringAsync());

            var token = await loginResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}