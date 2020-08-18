using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos;
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
        public async Task Can_CRUD_Application()
        {
            var appName = System.Guid.NewGuid().ToString();
            var persona = await GetPersonaAsync(Personas.AmphoraAdmin);

            var cApp = new CreateApplication
            {
                Name = appName,
                LogoutUrl = "http://localhost:11",
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
            appRead.LogoutUrl.Should().Be("http://localhost:11");
            appRead.Id.Should().Be(app.Id);
            appRead.Locations.Should().HaveCount(1);
            appRead.Locations[0].Origin.Should().Be(fakeOrigin);

            // list
            var listRes = await persona.Http.GetAsync("api/applications");
            var apps = await AssertHttpSuccess<CollectionResponse<Application>>(listRes);
            apps.Items.Should().HaveCountGreaterOrEqualTo(1);
            apps.Items.Should().Contain(_ => _.Id == appRead.Id);

            // update
            var updateModel = new UpdateApplication(app.Id);
            var newName = System.Guid.NewGuid().ToString();
            updateModel.Name = newName;
            updateModel.LogoutUrl = "https://localhost:91";
            updateModel.Locations = new List<CreateAppLocation>
            {
                new CreateAppLocation
                {
                    AllowedRedirectPaths = { "/login" },
                    Origin = "https://localhost:1010",
                    PostLogoutRedirects = { "https://google.com" }
                },
                new CreateAppLocation
                {
                    AllowedRedirectPaths = { "/login", "/login-two" },
                    Origin = fakeOrigin,
                    PostLogoutRedirects = { "https://bing.com" }
                },
                new CreateAppLocation
                {
                    AllowedRedirectPaths = { "/login", "/sign-in" },
                    Origin = "https://localhost:9000",
                    PostLogoutRedirects = { "https://amphoradata.com" }
                }
            };
            var updateRes = await persona.Http.PutAsJsonAsync($"api/applications/{app.Id}", updateModel);
            var updated = await AssertHttpSuccess<Application>(updateRes);
            updated.Name.Should().Be(newName);
            updated.LogoutUrl.Should().Be("https://localhost:91");
            updated.Locations.Should().HaveCount(3);

            // read again to check
            // read
            var readRes2 = await persona.Http.GetAsync($"api/applications/{app.Id}");
            var appRead2 = await AssertHttpSuccess<Application>(readRes2);
            appRead2.Name.Should().Be(newName);
            appRead2.Locations.Should().HaveCount(3);

            // delete
            var deleteRes = await persona.Http.DeleteAsync($"api/applications/{app.Id}");
            await AssertHttpSuccess(deleteRes, $"api/application/{app.Id}");
        }
    }
}
