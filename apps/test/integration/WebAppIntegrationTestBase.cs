using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public abstract class WebAppIntegrationTestBase : IntegrationTestBase
    {
        protected static class Personas
        {
            public const string Standard = nameof(Standard) + "@example.org";
            public const string StandardTwo = nameof(StandardTwo) + "@example.org";
            public const string AmphoraAdmin = nameof(AmphoraAdmin) + "@amphoradata.com";
            public const string Attacker = nameof(Attacker) + "@badactor.net";
            public const string Other = nameof(Other) + "@other.com";
        }

        private const string Password = "sjdbgBBHbdvklv984yt$$";

        private static Dictionary<string, Persona> personaCache = new Dictionary<string, Persona>();
        protected readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public WebAppIntegrationTestBase(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        protected async Task<Persona> GetPersonaAsync(string name = Personas.Standard)
        {
            if (!personaCache.ContainsKey(name))
            {
                await AddToCache(name);
            }

            return personaCache[name];
        }

        private async Task AddToCache(string name)
        {
            // first, check if we can login, so we need a client.
            var httpClient = _factory.CreateClient();
            if (!await TryLoginAndAddToCache(httpClient, name))
            {
                // then we gotta create the user
                await CreateAndAddToCache(httpClient, name);
            }
        }

        private async Task<bool> TryLoginAndAddToCache(HttpClient httpClient, string name)
        {
            if (await httpClient.GetTokenAsync(name, Password))
            {
                try
                {
                    var userInfo = await httpClient.LoadUserInfoAsync();
                    var org = await httpClient.GetOrganisationAsync(userInfo.OrganisationId);
                    personaCache[name] = new Persona(name, httpClient, userInfo, org);
                    return true;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"{name} failed to login, ${ex}");
                    await Task.Delay(100);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task CreateAndAddToCache(HttpClient httpClient, string email)
        {
            // special case when name == standard2
            if (email == Personas.StandardTwo)
            {
                // login with 1
                var client = personaCache[Personas.Standard];
                var (c, u, o) = await GetNewClientInOrg(client.Http, client.Organisation, email, httpClient);
                personaCache[email] = new Persona(email, c, u, o);
            }
            else if (email == Personas.Standard)
            {
                var (c, u, o) = await NewUser(email, Password, httpClient, TeamPlan);
                personaCache[email] = new Persona(email, c, u, o);
            }
            else
            {
                var (c, u, o) = await NewUser(email, Password, httpClient, FreePlan);
                personaCache[email] = new Persona(email, c, u, o);
            }
        }

        protected Plan.PlanTypes FreePlan => Plan.PlanTypes.Free;
        protected Plan.PlanTypes TeamPlan => Plan.PlanTypes.Team;
        protected Plan.PlanTypes InstitutionPlan => Plan.PlanTypes.Institution;

        private async Task<(HttpClient client, AmphoraUser user, Organisation org)> GetNewClientInOrg(
            HttpClient currentClient,
            Organisation org,
            string email,
            HttpClient client = null,
            int majorVersion = 0)
        {
            client ??= _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, majorVersion.ToString());

            // check if already invited.
            var getInvitesRes = await currentClient.GetAsync($"api/Organisations/{org.Id}/Invitations");
            await AssertHttpSuccess(getInvitesRes);
            var invitations = JsonConvert.DeserializeObject<List<Invitation>>(await getInvitesRes.Content.ReadAsStringAsync());
            if (!invitations.Any(_ => _.TargetEmail?.ToLower() == email.ToLower()))
            {
                // then invite needs to be created
                var inviteResponse = await currentClient.PostAsJsonAsync($"api/invitations/",
                    new Invitation { TargetEmail = email, TargetOrganisationId = org.Id });
                if (!inviteResponse.IsSuccessStatusCode)
                {
                    var message = await inviteResponse.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }

            var createUser = await client.CreateUserAsync(email, "type: " + this.GetType().ToString(), Password);
            var user = new AmphoraUser();
            user.UserName = createUser.UserName;
            user.Email = createUser.Email;
            user.Id = createUser.Id;
            var acceptDto = new AcceptInvitation { TargetOrganisationId = org.Id };
            var accept = await client.PostAsJsonAsync($"api/invitations/{org.Id}", acceptDto);
            accept.EnsureSuccessStatusCode();
            user.OrganisationId = org.Id;

            Assert.NotNull(user.Email);
            Assert.NotNull(user.Id);
            return (client, user, org);
        }

        private async Task<(HttpClient client, AmphoraUser user, Organisation org)> NewUser(
            string email,
            string password,
            HttpClient client,
            Plan.PlanTypes planType = Plan.PlanTypes.Free,
            int majorVersion = 0)
        {
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, majorVersion.ToString());
            var user = await client.CreateUserAsync(email, "type: " + this.GetType().ToString(), password);
            var org = await client.CreateOrganisationAsync("Integration: " + this.GetType().ToString());
            if (planType != Plan.PlanTypes.Free)
            {
                // update the plan
                await client.SetPlan(org.Id, planType);
            }

            return (client, user, org);
        }

        protected async Task<(HttpClient client, AmphoraUser user, Organisation org)> NewOrgAuthenticatedClientAsync(
            string domain = "AmphoraData.com",
            Plan.PlanTypes planType = Plan.PlanTypes.Free,
            int majorVersion = 0)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiVersion.HeaderName, majorVersion.ToString());
            var email = System.Guid.NewGuid().ToString() + "@" + domain;
            var user = await client.CreateUserAsync(email, "type: " + this.GetType().ToString());
            var org = await client.CreateOrganisationAsync("Integration: " + this.GetType().ToString());
            if (planType != Plan.PlanTypes.Free)
            {
                // update the plan
                await client.SetPlan(org.Id, planType);
            }

            return (client, user, org);
        }

        protected async Task DestroyAmphoraAsync(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            await AssertHttpSuccess(deleteResponse);
        }

        protected Task DestroyUserAsync(HttpClient client, AmphoraUser user)
        {
            // TODO: figure out a way to delete the user
            return Task.CompletedTask;
            // var response = await client.DeleteAsync($"api/users/{user.UserName}");
            // response.EnsureSuccessStatusCode();
        }

        protected async Task DestroyOrganisationAsync(HttpClient client, Organisation org)
        {
            var response = await client.DeleteAsync($"api/organisations/{org.Id}");
            await AssertHttpSuccess(response);
        }
    }
}