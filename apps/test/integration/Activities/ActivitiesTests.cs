using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Activities;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class ActivitiesTests : WebAppIntegrationTestBase
    {
        public ActivitiesTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task StandardUser_HappyPathWithRuns_ThenCanDelete()
        {
            var persona = await GetPersonaAsync();
            var name = nameof(StandardUser_HappyPathWithRuns_ThenCanDelete) + Guid.NewGuid().ToString(); // for unique name

            var createRes = await persona.Http.PostAsJsonAsync("api/activities", new CreateActivity(name));
            var activity = await AssertHttpSuccess<Activity>(createRes);
            activity.Name.Should().Be(name);
            activity.Id.Should().NotBeNull();

            // check we can get the activity
            var getRes = await persona.Http.GetAsync($"api/activities/{activity.Id}");
            var getActivity = await AssertHttpSuccess<Activity>(getRes);
            getActivity.Id.Should().Be(activity.Id, "because it should be the same acticity");
            getActivity.Name.Should().Be(activity.Name);

            // now start a run
            var startRunRes = await persona.Http.PostAsJsonAsync($"api/activities/{activity.Id}/Runs", new { });
            var run = await AssertHttpSuccess<Run>(startRunRes);

            run.Id.Should().NotBeNull();
            run.StartTime.Should().BeCloseTo(DateTime.UtcNow, 500, "because the run was just started");

            // create an amphora to reference
            var amphora = EntityLibrary.GetAmphoraDto(persona.Organisation.Id);
            var createAmphoraRes = await persona.Http.PostAsJsonAsync("api/amphorae", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createAmphoraRes);
            amphora.Id.Should().NotBeNull();

            // reference that amphora
            var referenceDto = new AmphoraReference
            {
                AmphoraId = amphora.Id,
                SignalsConsumed = 3,
                FilesConsumed = 5
            };
            var referenceRes = await persona.Http.PutAsJsonAsync($"api/activities/{activity.Id}/Runs/{run.Id}/amphorae/{amphora.Id}", referenceDto);
            var reference = await AssertHttpSuccess<AmphoraReference>(referenceRes);

            reference.FilesConsumed.Should().Be(referenceDto.FilesConsumed);
            reference.FilesProduced.Should().Be(referenceDto.FilesProduced);
            reference.SignalsConsumed.Should().Be(referenceDto.SignalsConsumed);
            reference.SignalsProduced.Should().Be(referenceDto.SignalsProduced);
            reference.AmphoraId.Should().Be(amphora.Id);

            // now complete the run
            var completeRes = await persona.Http.PostAsJsonAsync($"api/activities/{activity.Id}/Runs/{run.Id}", new UpdateRun(true));
            var completeRun = await AssertHttpSuccess<Run>(completeRes);
            completeRun.Id.Should().Be(run.Id);
            completeRun.Success.Should().BeTrue();
            completeRun.EndTime.Should().BeCloseTo(DateTime.UtcNow, 500, "because it just completed");
            completeRun.AmphoraReferences.Should().NotBeEmpty();
            completeRun.AmphoraReferences.Should().Contain(_ => _.AmphoraId == amphora.Id, "because we referenced that amphora");

            // now delete the activity
            var deleteRes = await persona.Http.DeleteAsync($"api/activities/{activity.Id}");
            await AssertHttpSuccess(deleteRes);

            // now check we can't get the activity
            getRes = await persona.Http.GetAsync($"api/activities/{activity.Id}");
            getRes.IsSuccessStatusCode.Should().BeFalse("because the activity should have been deleted.");
        }
    }
}