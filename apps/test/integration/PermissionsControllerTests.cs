using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.AccessControls;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    public class PermissionsControllerTests : WebAppIntegrationTestBase
    {
        public PermissionsControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanLoadPermissions_ForMineAndOthersAmphora()
        {
            // Arrange
            var standard = await GetPersonaAsync(Personas.Standard);
            var other = await GetPersonaAsync(Personas.Other);

            // both can make amphora
            var standardAmphora = EntityLibrary.GetAmphoraDto(standard.Organisation.Id);
            standardAmphora = await AssertHttpSuccess<DetailedAmphora>(await standard.Http.PostAsJsonAsync("api/amphorae", standardAmphora));
            var otherAmphora = EntityLibrary.GetAmphoraDto(other.Organisation.Id);
            otherAmphora = await AssertHttpSuccess<DetailedAmphora>(await other.Http.PostAsJsonAsync("api/amphorae", otherAmphora));

            var amphoraIds = new List<string> { standardAmphora.Id, otherAmphora.Id };
            // now both should get the permissions for the others amphora
            var p1 = new PermissionsRequest
            {
                AccessQueries = amphoraIds
                    .Select(_ => new AccessLevelQuery { AmphoraId = _, AccessLevel = (int)AccessLevels.ReadContents })
                    .ToList()
            };

            var standardPermissionResponse = await standard.Http.PostAsJsonAsync("api/permissions", p1);
            var res = await AssertHttpSuccess<PermissionsResponse>(standardPermissionResponse);
            res.Should().NotBeNull();
            res.AccessResponses.Should().HaveCount(2);
            var a = res.AccessResponses.FirstOrDefault(_ => _.AmphoraId == standardAmphora.Id);
            var b = res.AccessResponses.FirstOrDefault(_ => _.AmphoraId == otherAmphora.Id);
            a.IsAuthorized.Should().BeTrue();
            b.IsAuthorized.Should().BeFalse();
        }
    }
}