using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Feeds;
using Amphora.Api.Models.Feeds;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Feeds
{
    [Collection(nameof(ApiFixtureCollection))]
    public class FeedTests : WebAppIntegrationTestBase
    {
        public FeedTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Theory]
        [InlineData(Personas.AmphoraAdmin)]
        [InlineData(Personas.Standard)]
        [InlineData(Personas.StandardTwo)]
        [InlineData(Personas.Other)]
        public async Task Users_CanGetTheirFeed(string personaName)
        {
            var persona = await GetPersonaAsync(personaName);
            // create an amphora to ensure the feeds arent empty
            var a = EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            var createRes = await persona.Http.PostAsJsonAsync("api/amphorae", a);
            await AssertHttpSuccess(createRes);

            var feedRes = await persona.Http.GetAsync("api/feeds/v1");
            var feed = await AssertHttpSuccess<CollectionResponse<FeedItem>>(feedRes);
            feed.Items.Should().NotBeNullOrEmpty();
        }
    }
}