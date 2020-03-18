using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Tests.Setup;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(InMemoryIdentityFixtureCollection))]
    [Trait("Category", "Identity")]
    public class CRUDUserTests
    {
        private readonly InMemoryIdentityWebApplicationFactory factory;

        public CRUDUserTests(InMemoryIdentityWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CanCreateUser_InMemory_AsAnonymous_AndGetToken_AndDelete()
        {
            var client = factory.CreateClient();
            await Run_CanCreateUser_AsAnonymous_AndGetToken_AndDelete(client);
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public static async Task Run_CanCreateUser_AsAnonymous_AndGetToken_AndDelete(HttpClient client)
        {
            var email = Guid.NewGuid().ToString() + "@amphoradata.com";
            var fullName = Guid.NewGuid().ToString();
            var password = Guid.NewGuid().ToString() + "!A14";
            // assumed the user has been invited
            var user = new CreateAmphoraUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Password = password
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

            var tokenResponse = await client.PostAsJsonAsync("/api/token", new TokenRequest() { Username = email, Password = password });
            Assert.True(tokenResponse.IsSuccessStatusCode, "Content: " + await tokenResponse.Content.ReadAsStringAsync());
            var token = await tokenResponse.Content.ReadAsStringAsync();
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));

            // now check can delete
            var deleteRes = await client.DeleteAsync($"/api/users?userName={amphoraUser.UserName}");
            Assert.True(deleteRes.IsSuccessStatusCode, await deleteRes.Content.ReadAsStringAsync());
        }
    }
}
