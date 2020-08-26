using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Common.Models.Organisations.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    public class PlanSetAndGetTests : WebAppIntegrationTestBase
    {
        public PlanSetAndGetTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanGetAccountInformation()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var result = await adminClient.GetAsync($"api/Account");
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var account = JsonConvert.DeserializeObject<Api.Models.Dtos.Accounts.AccountInformation>(contents);
            Assert.NotNull(account);
        }

        [Theory]
        [InlineData(Plan.PlanTypes.Free)]
        [InlineData(Plan.PlanTypes.Team)]
        [InlineData(Plan.PlanTypes.Institution)]
        [InlineData(Plan.PlanTypes.PAYG)]
        [InlineData(Plan.PlanTypes.Glaze)]
        public async Task CanGetAndSetPlan(Plan.PlanTypes planType)
        {
            var (client, user, org) = await NewOrgAuthenticatedClientAsync();

            var result = await client.GetAsync($"api/Account/Plan");
            var contents = await result.Content.ReadAsStringAsync();
            var defaultPlan = await AssertHttpSuccess<PlanInformation>(result);
            defaultPlan.PlanType.Should().Be(Plan.PlanTypes.Free);
            defaultPlan.FriendlyName.Should().NotBeNullOrEmpty();

            var setResult = await client.PostAsJsonAsync($"api/Account/Plan?planType={planType}",
                new object());
            var setPlan = await AssertHttpSuccess<PlanInformation>(setResult);
            setPlan.Should().NotBeNull();
            setPlan.PlanType.Should().NotBeNull().And.Be(planType);
            setPlan.FriendlyName.Should().NotBeNullOrEmpty();

            result = await client.GetAsync($"api/Account/Plan");
            var getPlan = await AssertHttpSuccess<PlanInformation>(result);
            Assert.NotNull(getPlan);
            getPlan.PlanType.Should().Be(planType);
        }
    }
}
