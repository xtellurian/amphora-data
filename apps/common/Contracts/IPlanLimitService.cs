using System.Threading.Tasks;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;

namespace Amphora.Common.Contracts
{
    public interface IPlanLimitService
    {
        Task<bool> CanAddUser(OrganisationModel organisation);
        Task<PlanLimits> GetLimits(OrganisationModel organisation);
    }
}