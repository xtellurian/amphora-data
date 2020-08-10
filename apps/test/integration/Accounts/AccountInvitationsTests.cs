using System;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts.Memberships;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AccountInvitationsTests : WebAppIntegrationTestBase
    {
        public AccountInvitationsTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task GetInvitations_AsStandardUser()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);
            var p2 = await GetPersonaAsync(Personas.StandardTwo); // enure it exists

            var invitationReadRes = await persona.Http.GetAsync("api/account/invitations");
            var invitations = await AssertHttpSuccess<CollectionResponse<Invitation>>(invitationReadRes);

            invitations.Count.Should().NotBeNull().And.BeGreaterThan(0);
            invitations.Items.Should().NotBeEmpty();
            invitations.Items.Should().HaveCountGreaterOrEqualTo(1, "because standardtwo was invited");
            invitations.Items.Should().Contain(m =>
                string.Equals(m.TargetEmail, Personas.StandardTwo, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}