using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Amphora.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;
        private readonly Dictionary<HttpClient, ApplicationUser> userLoans;
        private readonly Dictionary<HttpClient, Organisation> orgLoans;

        public IntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
            this.userLoans = new Dictionary<HttpClient, ApplicationUser>();
            this.orgLoans = new Dictionary<HttpClient, Organisation>();
        }

        protected async Task<(HttpClient client, IApplicationUser user, Organisation org)> GetAuthenticatedClientAsync(RoleAssignment.Roles role = RoleAssignment.Roles.User, Organisation preOrg = null)
        {
            var client = _factory.CreateClient();
            var (user, org, password) = await client.CreateUserAsync(preOrg, role);
            userLoans[client] = user;
            orgLoans[client] = org;
            await client.GetTokenAsync(user, password);
            return (client, user, org);
        }

        protected async Task DestroyUserAsync(HttpClient client)
        {
            var user = userLoans[client];
            var response = await client.DeleteAsync($"api/users/{user.UserName}");
            response.EnsureSuccessStatusCode();
        }
        protected async Task DestroyOrganisationAsync(HttpClient client)
        {
            var org = orgLoans[client];
            var response = await client.DeleteAsync($"api/organisations/{org.OrganisationId}");
            response.EnsureSuccessStatusCode();
        }
    }
}