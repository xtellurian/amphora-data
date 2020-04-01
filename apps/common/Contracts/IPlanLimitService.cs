using System.Threading.Tasks;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;

namespace Amphora.Common.Contracts
{
    public interface IPlanLimitService
    {
        Task<PlanLimits> GetLimits(OrganisationModel organisation);
    }
}