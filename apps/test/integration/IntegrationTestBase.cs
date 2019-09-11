using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Amphora.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;
        private readonly Dictionary<HttpClient, ApplicationUser> userLoans;
        private readonly Dictionary<HttpClient, OrganisationModel> orgLoans;

        public IntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
            this.userLoans = new Dictionary<HttpClient, ApplicationUser>();
            this.orgLoans = new Dictionary<HttpClient, OrganisationModel>();
        }


        protected async Task<(HttpClient client, IApplicationUser user, OrganisationModel org)> GetNewClientInOrg(HttpClient currentClient, OrganisationModel org)
        {
            var client = _factory.CreateClient();
            var (user, password) = await client.CreateUserAsync();
            var inviteResponse = await currentClient.PostAsJsonAsync($"api/organisations/{org.OrganisationId}/invitations/", 
                new Invitation(user.Email));
            inviteResponse.EnsureSuccessStatusCode();
            var accept = await client.GetAsync($"api/organisations/{org.OrganisationId}/invitations");
            accept.EnsureSuccessStatusCode();
            inviteResponse.EnsureSuccessStatusCode();
            userLoans[client] = user;
            orgLoans[client] = org;
            return (client, user, org);
        }
        protected async Task<(HttpClient client, IApplicationUser user, OrganisationModel org)> NewOrgAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();
            var (user, password) = await client.CreateUserAsync();
            var org = await client.CreateOrganisationAsync();
            userLoans[client] = user;
            orgLoans[client] = org;
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