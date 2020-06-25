using System.Threading.Tasks;
using Amphora.Api.Models.Dtos.Geo;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class GeoTests : WebAppIntegrationTestBase
    {
        public GeoTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task FuzzySearchTests()
        {
            // THIS ENDPOINT IS BECOMING OBSOLETE SOMETIME
            // Arrange
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            // Act
            var response = await client.GetAsync("/api/location/fuzzy?query=sydney");

            // Assert
            await AssertHttpSuccess(response);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Common.Models.AzureMaps.FuzzySearchResponse>(content);
            Assert.NotNull(obj);
            Assert.NotEmpty(obj.Results);
        }

        [Theory]
        [InlineData("sydney")]
        [InlineData("melbourne")]
        public async Task FuzzySearch_ReturnsSomething(string query)
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);

            // Act
            var response = await persona.Http.GetAsync($"/api/geo/search/fuzzy?query={query}");

            // Assert
            var fuzzy = await AssertHttpSuccess<FuzzySearchResponse>(response);
            fuzzy.Results.Should().NotBeNull().And.NotBeEmpty();
            fuzzy.Summary.Query.Should().Be(query);
        }
    }
}