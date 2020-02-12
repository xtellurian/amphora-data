using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
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

        protected async Task<(HttpClient client, AmphoraUser user, Organisation org)> GetNewClientInOrg(
            HttpClient currentClient,
            Organisation org,
            int majorVersion = 0)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, majorVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var inviteResponse = await currentClient.PostAsJsonAsync($"api/invitations/",
                new Invitation { TargetEmail = email, TargetOrganisationId = org.Id });
            inviteResponse.EnsureSuccessStatusCode();
            var (user, password) = await client.CreateUserAsync(email, "type: " + this.GetType().ToString());
            var acceptDto = new AcceptInvitation { TargetOrganisationId = org.Id };
            var accept = await client.PostAsJsonAsync($"api/invitations/{org.Id}", acceptDto);
            accept.EnsureSuccessStatusCode();
            inviteResponse.EnsureSuccessStatusCode();
            user.OrganisationId = org.Id;
            return (client, user, org);
        }

        protected async Task<(HttpClient client, AmphoraUser user, Organisation org)> NewOrgAuthenticatedClientAsync(
            int majorVersion = 0)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, majorVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var (user, password) = await client.CreateUserAsync(email, "type: " + this.GetType().ToString());
            var org = await client.CreateOrganisationAsync("Integration: " + this.GetType().ToString());
            return (client, user, org);
        }

        protected async Task DestroyAmphoraAsync(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        protected async Task DestroyUserAsync(HttpClient client, AmphoraUser user)
        {
            var response = await client.DeleteAsync($"api/users/{user.UserName}");
            response.EnsureSuccessStatusCode();
        }

        protected async Task DestroyOrganisationAsync(HttpClient client, Organisation org)
        {
            var response = await client.DeleteAsync($"api/organisations/{org.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}