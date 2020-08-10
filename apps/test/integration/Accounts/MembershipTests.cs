using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts.Memberships;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    public class MembershipTests : WebAppIntegrationTestBase
    {
        public MembershipTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task GetMemberships_AsStandardUser()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);
            var p2 = await GetPersonaAsync(Personas.StandardTwo); // enure it exists

            var membershipReadRes = await persona.Http.GetAsync("api/account/memberships");
            var memberships = await AssertHttpSuccess<CollectionResponse<Membership>>(membershipReadRes);

            memberships.Count.Should().NotBeNull().And.BeGreaterThan(0);
            memberships.Items.Should().NotBeEmpty();
            memberships.Items.Should().HaveCount(2);
            memberships.Items.Should().Contain(m => m.Username == Personas.Standard);
            memberships.Items.Should().Contain(m => m.Username == Personas.StandardTwo);
        }
    }
}