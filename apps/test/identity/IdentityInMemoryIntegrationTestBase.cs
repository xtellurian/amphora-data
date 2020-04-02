using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Tests.Setup;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    public class IdentityInMemoryIntegrationTestBase : IntegrationTestBase
    {
        protected readonly InMemoryIdentityWebApplicationFactory _factory;

        public IdentityInMemoryIntegrationTestBase(InMemoryIdentityWebApplicationFactory factory)
        {
            _factory = factory;
        }

        protected async Task<(HttpClient client, AmphoraUser user)> NewAuthenticatedUser(string email = null)
        {
            var client = _factory.CreateClient();
            email ??= Guid.NewGuid().ToString() + "@amphoradata.com";
            var fullName = Guid.NewGuid().ToString();
            var password = Guid.NewGuid().ToString() + "!A14";
            // assumed the user has been invited
            var user = new CreateAmphoraUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Password = password,
                ConfirmPassword = password
            };
            var requestPath = "api/users";

            var response = await client.PostAsJsonAsync(requestPath, user);

            Assert.True(response.IsSuccessStatusCode, "Content: " + await response.Content.ReadAsStringAsync());
            var amphoraUser = JsonConvert.DeserializeObject<AmphoraUser>(await response.Content.ReadAsStringAsync());

            Assert.NotNull(amphoraUser.Email);
            Assert.Equal(user.Email, amphoraUser.Email);
            Assert.NotNull(amphoraUser.UserName);
            Assert.Equal(user.UserName, amphoraUser.UserName);
            Assert.NotNull(amphoraUser.FullName);
            Assert.Equal(user.FullName, amphoraUser.FullName);

            var tokenResponse = await client.PostAsJsonAsync("/api/token", new LoginRequest() { Username = email, Password = password });
            Assert.True(tokenResponse.IsSuccessStatusCode, "Content: " + await tokenResponse.Content.ReadAsStringAsync());
            var token = await tokenResponse.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return (client, amphoraUser);
        }
    }
}