using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class SearchTests : WebAppIntegrationTestBase
    {
        public SearchTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
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

        [Fact]
        public async Task SearchAmphora_CanPaginate()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);
            await PopulateAmphora(persona);

            var paginateRes = await persona.Http.GetAsync("api/search-v2/amphorae?skip=2&take=3");
            var result = await AssertHttpSuccess<SearchResponse<BasicAmphora>>(paginateRes);
            result.Items.Should().HaveCount(3, "because we set take to 3");
        }

        [Fact]
        public async Task SearchAmphora_CanChooseOrgId()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);
            await PopulateAmphora(persona);

            var paginateRes = await persona.Http.GetAsync($"api/search-v2/amphorae?orgId={persona.Organisation.Id}");
            var result = await AssertHttpSuccess<SearchResponse<BasicAmphora>>(paginateRes);
            foreach (var r in result.Items)
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

            var url = $"api/search-v2/amphorae?orgId={admin.Organisation.Id}&labels={labels}";
            var paginateRes = await other.Http.GetAsync(url);
            var result = await AssertHttpSuccess<SearchResponse<BasicAmphora>>(paginateRes);
            foreach (var amphora in result.Items)
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

            var searchRes = await client.GetAsync($"api/search-v2/organisations?term={org.Name}");
            var content = await searchRes.Content.ReadAsStringAsync();
            var result = await AssertHttpSuccess<SearchResponse<Organisation>>(searchRes);
            result.Items.Should().NotBeNullOrEmpty();
        }
    }
}