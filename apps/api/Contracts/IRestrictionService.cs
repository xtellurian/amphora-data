using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Contracts
{
    public interface IRestrictionService
    {
        Task<EntityOperationResult<RestrictionModel>> CreateAsync(ClaimsPrincipal principal, RestrictionModel restriction);
        Task<EntityOperationResult<RestrictionModel>> DeleteAsync(ClaimsPrincipal principal, string restrictionId);
    }
}