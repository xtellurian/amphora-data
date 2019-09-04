using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Amphora.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;
        private readonly Dictionary<HttpClient, ApplicationUser> clientLoans;

        public IntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
            this.clientLoans = new Dictionary<HttpClient, ApplicationUser>();
        }

        protected async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();
            var (user, password) = await client.CreateUserAsync();
            clientLoans[client] = user;
            await client.GetTokenAsync(user, password);
            return client;
        }

        protected async Task DestroyUser(HttpClient client)
        {
            var user = clientLoans[client];
            var response = await client.DeleteAsync($"api/users/{user.UserName}");
            response.EnsureSuccessStatusCode();
        }
    }
}