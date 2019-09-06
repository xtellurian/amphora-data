using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;
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
        public static async Task<(ApplicationUser User, Organisation Org, string Password)> CreateUserAsync(
            this HttpClient client,
            Organisation org = null,
            RoleAssignment.Roles role = RoleAssignment.Roles.User)
        {
            client.AddCreateToken();
            // first, create an organisation
            if(org == null)
            {
                org = await client.CreateOrganisationAsync();
            }

            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                OrganisationId = org.OrganisationId
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/users?role={role}", content);
            var password = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299

            return (User: user, Org: org, Password: password);
        }

        public static async Task<Organisation> CreateOrganisationAsync(this HttpClient client)
        {
            var a = Helpers.EntityLibrary.GetOrganisation();
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/organisations", requestBody);
            var createResponseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var org = JsonConvert.DeserializeObject<Organisation>(createResponseContent);
            return org;
        }

        public static async Task GetTokenAsync(this HttpClient client, ApplicationUser user, string password)
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