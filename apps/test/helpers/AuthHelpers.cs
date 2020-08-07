using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Helpers
{
    public static class AuthHelpers
    {
        public const string Password = "sjdbgBBHbdvklv984yt$$";
        private const string PhoneNumber = "0412 345 678";
        public static async Task<AmphoraUser> CreateUserAsync(
            this HttpClient client,
            string email,
            string fullName,
            string password = null)
        {
            password ??= System.Guid.NewGuid().ToString() + "!A1";
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

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/users", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"[{email}] [{response.StatusCode}] Content:  + {responseContent}");
            var createdUser = JsonConvert.DeserializeObject<AmphoraUser>(responseContent);
            var loginRequest = new LoginRequest()
            {
                Password = password,
                Username = user.UserName,
                Claims = new List<LoginClaim> { new LoginClaim(Claims.Purchase, "") }
            };
            if (await client.GetTokenAsync(loginRequest))
            {
                return createdUser;
            }
            else
            {
                throw new System.InvalidOperationException("Failed to login");
            }
        }

        public static async Task<AmphoraUser> LoadUserInfoAsync(this HttpClient client)
        {
            var res = await client.GetAsync("api/users/self");
            if (res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AmphoraUser>(await res.Content.ReadAsStringAsync());
            }
            else
            {
                throw new System.Exception("Failed to get user info");
            }
        }

        public static async Task<Organisation> CreateOrganisationAsync(this HttpClient client, string testName)
        {
            // check if already in org before trying to create one (autocreate effect)
            var selfRes = await client.GetAsync("api/users/self");
            selfRes.IsSuccessStatusCode.Should().BeTrue();
            var self = JsonConvert.DeserializeObject<AmphoraUser>(await selfRes.Content.ReadAsStringAsync());
            Organisation organisation;
            if (self.OrganisationId is null)
            {
                var a = Helpers.EntityLibrary.GetOrganisationDto(testName);
                var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/organisations", requestBody);
                var createResponseContent = await response.Content.ReadAsStringAsync();
                Assert.True(response.IsSuccessStatusCode, "Content: " + await response.Content.ReadAsStringAsync());
                organisation = JsonConvert.DeserializeObject<Organisation>(createResponseContent);
            }
            else
            {
                var orgRes = await client.GetAsync($"api/organisations/{self.OrganisationId}");
                orgRes.IsSuccessStatusCode.Should().BeTrue();
                organisation = JsonConvert.DeserializeObject<Organisation>(await orgRes.Content.ReadAsStringAsync());
            }

            return organisation;
        }

        public static async Task<Organisation> GetOrganisationAsync(this HttpClient client, string organisationId)
        {
            var res = await client.GetAsync($"api/Organisations/{organisationId}");
            if (res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Organisation>(await res.Content.ReadAsStringAsync());
            }
            else
            {
                throw new System.Exception($"Failed to get organisation info from response: {res}");
            }
        }

        public static async Task SetPlan(this HttpClient client, string orgId, Common.Models.Organisations.Accounts.Plan.PlanTypes planType)
        {
            var setResult = await client.PostAsJsonAsync($"api/Organisations/{orgId}/Account/Plan?planType={planType}",
                    new object());
            setResult.EnsureSuccessStatusCode();
        }

        public static async Task<bool> GetTokenAsync(this HttpClient client, Persona p, IEnumerable<LoginClaim> claims = null)
        {
            var loginReq = new LoginRequest(p.User.UserName, Password, claims);
            return await client.GetTokenAsync(loginReq);
        }

        /// <summary>
        /// Trys to get a token. Returns whether successful.
        /// </summary>
        public static async Task<bool> GetTokenAsync(this HttpClient client, LoginRequest loginRequest)
        {
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("api/authentication/request", loginContent);
            if (loginResponse.IsSuccessStatusCode)
            {
                var token = await loginResponse.Content.ReadAsStringAsync();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            else
            {
                var responseContent = await loginResponse.Content.ReadAsStringAsync();
                return false;
            }
        }
    }
}