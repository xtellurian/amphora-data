using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    [Trait("Category", "Identity")]
    public class CRUDUserTests : IdentityIntegrationTestBase
    {
        public CRUDUserTests(WebApplicationFactory<Amphora.Identity.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanCreateUser_InMemory_AsAnonymous_AndGetToken_AndDelete()
        {
            var client = _factory.CreateClient();
            var email = Guid.NewGuid().ToString() + "@amphoradata.com";
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
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));

            var tokenResponseWithPurchase = await client.PostAsJsonAsync("/api/token",
                new LoginRequest()
                {
                    Username = email,
                    Password = password,
                    Claims = new List<LoginClaim>
                    {
                        new LoginClaim("scope", Claims.Purchase)
                    }
                });

            Assert.True(tokenResponseWithPurchase.IsSuccessStatusCode, "Content: " + await tokenResponseWithPurchase.Content.ReadAsStringAsync());
            var tokenWithPurchase = await tokenResponseWithPurchase.Content.ReadAsStringAsync();
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));
            var jwt = GetPrincipal(tokenWithPurchase);
            jwt.Claims.Should().NotBeNullOrEmpty();
            jwt.Claims.Should().Contain(_ => _.Type == "scope" && _.Value == Claims.Purchase);

            // now check can delete
            var deleteRes = await client.DeleteAsync($"/api/users?userName={amphoraUser.UserName}");
            Assert.True(deleteRes.IsSuccessStatusCode, await deleteRes.Content.ReadAsStringAsync());
        }

        private JwtSecurityToken GetPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
    }
}
