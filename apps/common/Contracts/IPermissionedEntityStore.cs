using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;

namespace Amphora.Common.Contracts
{
    public interface IPermissionedEntityStore<T> where T : EntityBase
    {
        Task<EntityOperationResult<T>> CreateAsync(ClaimsPrincipal principal, T model);
        Task<EntityOperationResult<T>> ReadAsync(ClaimsPrincipal principal, string id);
        Task<EntityOperationResult<T>> UpdateAsync(ClaimsPrincipal principal, T model);
        Task<EntityOperationResult<T>> DeleteAsync(ClaimsPrincipal principal, T model);
    }
}