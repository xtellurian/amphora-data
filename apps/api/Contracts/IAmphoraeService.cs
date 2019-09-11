using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        IEntityStore<Common.Models.AmphoraModel> AmphoraStore { get; }

        Task<EntityOperationResult<Common.Models.AmphoraModel>> CreateAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel model);
        Task<EntityOperationResult<Common.Models.AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity);
        Task<EntityOperationResult<Common.Models.AmphoraModel>> ReadAsync(ClaimsPrincipal principal, string id, string orgId = null);
        Task<EntityOperationResult<Common.Models.AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, Common.Models.AmphoraModel entity);
    }
}