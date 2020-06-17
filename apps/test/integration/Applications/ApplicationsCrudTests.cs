using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Applications;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Applications
{
    [Collection(nameof(ApiFixtureCollection))]
    public class ApplicationsCrudTests : WebAppIntegrationTestBase
    {
        private string fakeOrigin = "http://localhost:9999";
        public ApplicationsCrudTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Can_CreateReadDelete_Application()
        {
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);

            var cApp = new CreateApplication
            {
                Name = "My Applicaton",
                Locations = new List<CreateAppLocation>
                {
                    new CreateAppLocation
                    {
                        AllowedRedirectPaths = { "/login" },
                        Origin = fakeOrigin,
                        PostLogoutRedirects = { "https://google.com" }
                    }
                }
            };

            var createRes = await persona.Http.PostAsJsonAsync("api/applications", cApp);
            var app = await AssertHttpSuccess<Application>(createRes, "api/applications");
            app.Name.Should().Be(cApp.Name);
            app.Id.Should().NotBeNull();
            app.Locations.Should().HaveCount(1);
            app.Locations[0].Origin.Should().Be(fakeOrigin);

            // read
            var readRes = await persona.Http.GetAsync($"api/applications/{app.Id}");
            var appRead = await AssertHttpSuccess<Application>(readRes);
            appRead.Name.Should().Be(app.Name);
            appRead.Id.Should().Be(app.Id);
            appRead.Locations.Should().HaveCount(1);
            appRead.Locations[0].Origin.Should().Be(fakeOrigin);

            // delete
            var deleteRes = await persona.Http.DeleteAsync($"api/applications/{app.Id}");
            deleteRes.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
