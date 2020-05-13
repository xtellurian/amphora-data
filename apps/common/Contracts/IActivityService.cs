using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Activities;

namespace Amphora.Common.Contracts
{
    public interface IActivityService
    {
        Task<EntityOperationResult<ActivityModel>> CreateAsync(ClaimsPrincipal principal, ActivityModel activity);
        Task<EntityOperationResult<ActivityModel>> DeleteAsync(ClaimsPrincipal principal, ActivityModel activity);
        Task<EntityOperationResult<ActivityModel>> ReadAsync(ClaimsPrincipal principal, string idOrName);
        Task<EntityOperationResult<ActivityModel>> UpdateNameAsync(ClaimsPrincipal principal, string id, string name);
    }
}