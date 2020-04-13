using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Services.Plans;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class PlanLimitServiceTests : UnitTestBase
    {
        [Theory]
        [InlineData(Plan.PlanTypes.Free, 10)]
        [InlineData(Plan.PlanTypes.Team, 100)]
        [InlineData(Plan.PlanTypes.Institution, 250)]
        public async Task ForPlans_OneUser_MaxStorage(Plan.PlanTypes planType, long storageGb)
        {
            var org = new OrganisationModel
            {
                Memberships = new List<Membership>
                {
                    new Membership(System.Guid.NewGuid().ToString())
                }
            };

            org.Account.Plan.PlanType = planType;

            var sut = new PlanLimitService();

            var limits = await sut.GetLimits(org);
            Assert.True(limits.MaxStorageInBytes >= storageGb);
        }

        [Theory]
        [InlineData(Plan.PlanTypes.Free, 1)]
        [InlineData(Plan.PlanTypes.Team, 25)]
        [InlineData(Plan.PlanTypes.Institution, int.MaxValue)]
        public async Task ForPlans_MaxUsers(Plan.PlanTypes planType, long maxUsers)
        {
            var org = new OrganisationModel
            {
                Memberships = new List<Membership>
                {
                    new Membership(System.Guid.NewGuid().ToString())
                }
            };

            org.Account.Plan.PlanType = planType;

            var sut = new PlanLimitService();

            var limits = await sut.GetLimits(org);
            Assert.True(limits.MaxUsers >= maxUsers);
        }

        [Theory]
        [InlineData(Plan.PlanTypes.Team)]
        [InlineData(Plan.PlanTypes.Institution)]
        public async Task SomePlans_CanAddUpTo5Users(Plan.PlanTypes planType)
        {
            var org = new OrganisationModel();

            org.Account.Plan.PlanType = planType;

            var sut = new PlanLimitService();

            for (var i = 0; i < 5; i++)
            {
                Assert.True(await sut.CanAddUser(org));
                org.Memberships.Add(new Membership(System.Guid.NewGuid().ToString()));
            }
        }
    }
}