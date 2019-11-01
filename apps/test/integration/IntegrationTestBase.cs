using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Users;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Amphora.Api.Models.Dtos.Platform;

namespace Amphora.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public IntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        protected async Task<(HttpClient client, UserDto user, OrganisationDto org)> GetNewClientInOrg(HttpClient currentClient, OrganisationDto org)
        {
            var client = _factory.CreateClient();
            var email = System.Guid.NewGuid().ToString() + "@amphoradata.com";
            var inviteResponse = await currentClient.PostAsJsonAsync($"api/invitations/",
                new InvitationDto{TargetEmail = email, TargetOrganisationId = org.Id});
            inviteResponse.EnsureSuccessStatusCode();
            var (user, password) = await client.CreateUserAsync(email, "type: " + this.GetType().ToString());
            var acceptDto = new AcceptInvitationDto{TargetOrganisationId = org.Id};
            var accept = await client.PostAsJsonAsync($"api/invitations/{org.Id}", acceptDto);
            accept.EnsureSuccessStatusCode();
            inviteResponse.EnsureSuccessStatusCode();
            user.OrganisationId = org.Id;
            return (client, user, org);
        }
        protected async Task<(HttpClient client, UserDto user, OrganisationDto org)> NewOrgAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();
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
        protected async Task DestroyUserAsync(HttpClient client, UserDto user)
        {
            var response = await client.DeleteAsync($"api/users/{user.UserName}");
            response.EnsureSuccessStatusCode();
        }
        protected async Task DestroyOrganisationAsync(HttpClient client, OrganisationDto org)
        {
            var response = await client.DeleteAsync($"api/organisations/{org.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}