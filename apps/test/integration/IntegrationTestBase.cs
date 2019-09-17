using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Amphora.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public IntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        protected async Task<(HttpClient client, IApplicationUser user, OrganisationModel org)> GetNewClientInOrg(HttpClient currentClient, OrganisationModel org)
        {
            var client = _factory.CreateClient();
            var (user, password) = await client.CreateUserAsync("type: " + this.GetType().ToString());
            var inviteResponse = await currentClient.PostAsJsonAsync($"api/organisations/{org.OrganisationId}/invitations/",
                new Invitation(user.Email));
            inviteResponse.EnsureSuccessStatusCode();
            var accept = await client.GetAsync($"api/organisations/{org.OrganisationId}/invitations");
            accept.EnsureSuccessStatusCode();
            inviteResponse.EnsureSuccessStatusCode();
            user.OrganisationId = org.OrganisationId;
            return (client, user, org);
        }
        protected async Task<(HttpClient client, IApplicationUser user, OrganisationModel org)> NewOrgAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();
            var (user, password) = await client.CreateUserAsync("type: " + this.GetType().ToString());
            var org = await client.CreateOrganisationAsync("Integration: " + this.GetType().ToString());
            return (client, user, org);
        }

        protected async Task DestroyAmphoraAsync(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            deleteResponse.EnsureSuccessStatusCode();
        }
        protected async Task DestroyUserAsync(HttpClient client, IApplicationUser user)
        {
            var response = await client.DeleteAsync($"api/users/{user.UserName}");
            response.EnsureSuccessStatusCode();
        }
        protected async Task DestroyOrganisationAsync(HttpClient client, OrganisationModel org)
        {
            var response = await client.DeleteAsync($"api/organisations/{org.OrganisationId}");
            response.EnsureSuccessStatusCode();
        }
    }
}