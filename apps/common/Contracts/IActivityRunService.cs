using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Activities;

namespace Amphora.Common.Contracts
{
    public interface IActivityRunService
    {
        Task<EntityOperationResult<ActivityRunModel>> FinishRunAsync(ClaimsPrincipal principal,
                                                                     ActivityModel activity,
                                                                     ActivityRunModel run,
                                                                     bool success = true);
        Task<EntityOperationResult<ActivityRunModel>> ReferenceAmphoraAsync(ClaimsPrincipal principal,
                                                                            ActivityModel activity,
                                                                            ActivityRunModel run,
                                                                            ActivityAmphoraReference amphoraRef);
        Task<EntityOperationResult<ActivityRunModel>> StartRunAsync(ClaimsPrincipal principal, ActivityModel activity);
    }
}