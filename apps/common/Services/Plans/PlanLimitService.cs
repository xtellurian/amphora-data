using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Platform;

namespace Amphora.Common.Services.Plans
{
    public class PlanLimitService : IPlanLimitService
    {
        public Task<PlanLimits> GetLimits(OrganisationModel organisation)
        {
            if (organisation is null)
            {
                throw new System.ArgumentNullException(nameof(organisation));
            }

            var plan = organisation?.Account?.Plan ?? new Plan();
            var nUsers = organisation?.Memberships?.Count ?? 1;
            switch (plan.PlanType)
            {
                case Plan.PlanTypes.Free:
                    return Task.FromResult(FreePlanLimits());
                case Plan.PlanTypes.Team:
                    return Task.FromResult(TeamPlanLimits(nUsers));
                case Plan.PlanTypes.Institution:
                    return Task.FromResult(InstitutionPlanLimits(nUsers));
                default:
                    return Task.FromResult(FreePlanLimits());
            }
        }

        public async Task<bool> CanAddUser(OrganisationModel organisation)
        {
            var limits = await GetLimits(organisation);
            var memberCount = organisation.Memberships?.Count ?? 1;
            return memberCount < limits.MaxUsers;
        }

        public async Task<bool> CanAddNewFile(OrganisationModel organisation)
        {
            var limits = await GetLimits(organisation);
            var current = organisation.Cache?.TotalAmphoraeFileSize?.Value ?? 0;
            return current < limits.MaxStorageInBytes;
        }

        private PlanLimits FreePlanLimits()
        {
            return new PlanLimits(10 * Units.GB, 2);
        }

        private PlanLimits TeamPlanLimits(int nUsers)
        {
            return new PlanLimits(100 * nUsers * Units.GB, 10);
        }

        private PlanLimits InstitutionPlanLimits(int nUsers)
        {
            return new PlanLimits(100 * nUsers * Units.GB, int.MaxValue);
        }
    }
}