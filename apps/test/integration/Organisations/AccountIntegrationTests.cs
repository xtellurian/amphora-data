using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration.Organisations
{
    [Collection(nameof(ApiFixtureCollection))]
    public class AccountIngregrationTests : WebAppIntegrationTestBase
    {
        public AccountIngregrationTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CanGetAccountInformation()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var result = await adminClient.GetAsync($"api/Organisations/{adminOrg.Id}/Account");
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var account = JsonConvert.DeserializeObject<Api.Models.Dtos.Organisations.Account>(contents);
            Assert.NotNull(account);
        }

        [Fact]
        public async Task CanGetAndSetPlan()
        {
            var (adminClient, adminUser, adminOrg) = await NewOrgAuthenticatedClientAsync();

            var result = await adminClient.GetAsync($"api/Organisations/{adminOrg.Id}/Account/Plan");
            var contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            var plan = JsonConvert.DeserializeObject<Api.Models.Dtos.Organisations.PlanInformation>(contents);
            Assert.NotNull(plan);
            Assert.NotNull(plan.FriendlyName);

            var setResult = await adminClient.PostAsJsonAsync($"api/Organisations/{adminOrg.Id}/Account/Plan?planType={Plan.PlanTypes.Team}",
                new object());
            var setContents = await setResult.Content.ReadAsStringAsync();
            await AssertHttpSuccess(setResult);
            plan = JsonConvert.DeserializeObject<Api.Models.Dtos.Organisations.PlanInformation>(setContents);
            Assert.NotNull(plan);
            Assert.NotNull(plan.FriendlyName);
            Assert.Equal(Plan.PlanTypes.Team, plan.PlanType);

            result = await adminClient.GetAsync($"api/Organisations/{adminOrg.Id}/Account/Plan");
            contents = await result.Content.ReadAsStringAsync();
            await AssertHttpSuccess(result);

            plan = JsonConvert.DeserializeObject<Api.Models.Dtos.Organisations.PlanInformation>(contents);
            Assert.NotNull(plan);
            Assert.Equal(Plan.PlanTypes.Team, plan.PlanType);
        }
    }
}
