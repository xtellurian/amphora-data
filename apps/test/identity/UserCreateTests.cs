using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    public class CRUDUserTests
    {
        private readonly WebApplicationFactory<Amphora.Identity.Startup> factory;

        public CRUDUserTests(WebApplicationFactory<Amphora.Identity.Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CanCreateUser_AsAnonymous_AndGetToken_AndDelete()
        {
            var client = factory.CreateClient();
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
