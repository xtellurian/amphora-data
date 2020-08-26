using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.BackwardsCompat
{
    [Collection(nameof(ApiFixtureCollection))]
    public class SearchTests_BackwardsCompatV1 : WebAppIntegrationTestBase
    {
        public SearchTests_BackwardsCompatV1(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        private async Task PopulateAmphora(Persona persona, [CallerMemberName] string testName = null)
        {
            for (var i = 0; i <= 5; i++)
            {
                var amphora = EntityLibrary.GetAmphoraDto(persona.Organisation.Id, testName);
                if (i % 3 == 0)
                {
                    amphora.Labels = "a";
                }

                if (i % 4 == 0)
                {
                    amphora.Labels += "b";
                }

                var res = await persona.Http.PostAsJsonAsync("api/amphorae", amphora);
                await AssertHttpSuccess(res);
            }

            // now trigger the indexing, if we are an admin
            if (persona.User.Email == Personas.AmphoraAdmin)
            {
                var indexRes = await persona.Http.PostAsJsonAsync("api/search/indexers", new { });
                if (!indexRes.IsSuccessStatusCode)
                {
                    Console.WriteLine("Waiting 30s for reindexation");
                    await Task.Delay(30 * 1000);
                    Console.WriteLine("Retriggering index");
                    await persona.Http.PostAsJsonAsync("api/search/indexers", new { });
                }
            }
        }

        [Theory]
        [InlineData("api/search/amphorae/byOrganisation")]
        public async Task SearchAmphorae_ByOrgIdAsUser_ForTeamPlan(string url)
        {
            // Arrange
            var client = await GetPersonaAsync();
            var otherClient = await GetPersonaAsync(Personas.StandardTwo);
            var a = Helpers.EntityLibrary.GetAmphoraDto(client.Organisation.Id);
            var createResponse = await client.Http.PostAsJsonAsync("api/amphorae", a);
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            await AssertHttpSuccess(createResponse);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(createResponseContent);

            // Act
            var response = await otherClient.Http.GetAsync($"{url}?orgId={client.Organisation.Id}");
            await AssertHttpSuccess(response);
            var content = await response.Content.ReadAsStringAsync();
            var amphorae = JsonConvert.DeserializeObject<List<DetailedAmphora>>(content);
        }

        [Theory]
        [InlineData("api/search/amphorae/byLocation")]
        public async Task SearchAmphorae_ByLocation(string url)
        {
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            var rnd = new Random();
            var lat = rnd.Next(90);
            var lon = rnd.Next(90);
            var term = string.Empty;
            // let's create an amphorae
            var a = EntityLibrary.GetAmphoraDto(org.Id, nameof(SearchAmphorae_ByLocation));
            a.Lat = lat;
            a.Lon = lon;
            var content = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync("api/amphorae", content);
            a = JsonConvert.DeserializeObject<DetailedAmphora>(await createResponse.Content.ReadAsStringAsync());
            await AssertHttpSuccess(createResponse);
            var response = await client.GetAsync($"{url}?lat={lat}&lon={lon}&dist=10");
            var responseContent = await response.Content.ReadAsStringAsync();
            await AssertHttpSuccess(response);

            var responseList = JsonConvert.DeserializeObject<List<DetailedAmphora>>(responseContent);
        }

        [Fact]
        public async Task SearchAmphora_CanPaginate()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);
            await PopulateAmphora(persona);

            var paginateRes = await persona.Http.GetAsync("api/search/amphorae?skip=2&take=3");
            var results = await AssertHttpSuccess<List<BasicAmphora>>(paginateRes);
            results.Should().HaveCount(3, "because we set take to 3");
        }

        [Fact]
        public async Task SearchAmphora_CanChooseOrgId()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);
            await PopulateAmphora(persona);

            var paginateRes = await persona.Http.GetAsync($"api/search/amphorae?orgId={persona.Organisation.Id}");
            var results = await AssertHttpSuccess<List<BasicAmphora>>(paginateRes);
            foreach (var r in results)
            {
                r.OrganisationId.Should().Be(persona.Organisation.Id);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("a,b")]
        public async Task SearchAmphora_CanChooseOrgId_WithLabels(string labels)
        {
            var labelsList = labels.Split(',').Where(_ => !string.IsNullOrEmpty(_)).ToList();
            var admin = await GetPersonaAsync(Personas.AmphoraAdmin);
            var other = await GetPersonaAsync(Personas.Other);

            await PopulateAmphora(admin);
            await PopulateAmphora(other);

            var url = $"api/search/amphorae?orgId={admin.Organisation.Id}&labels={labels}";
            var paginateRes = await other.Http.GetAsync(url);
            var results = await AssertHttpSuccess<List<BasicAmphora>>(paginateRes);
            foreach (var amphora in results)
            {
                amphora.OrganisationId.Should().Be(admin.Organisation.Id, "because we filtered by orgId");
                foreach (var l in labelsList)
                {
                    labelsList.Intersect(amphora.Labels.Split(','))
                        .Should().NotBeNullOrEmpty("because there should be some label intersection");
                }
            }
        }

        [Fact]
        public async Task SearchOrganisations_ByName()
        {
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();
            // now we have an org, it should show up in the search.

            var searchRes = await client.GetAsync($"api/search/organisations?term={org.Name}");
            var content = await searchRes.Content.ReadAsStringAsync();
            await AssertHttpSuccess(searchRes);
            var orgs = JsonConvert.DeserializeObject<List<Organisation>>(content);

            Assert.NotNull(orgs);
        }
    }
}