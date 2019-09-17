using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        IEntityStore<AmphoraModel> AmphoraStore { get; }

        Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model);
        Task<EntityOperationResult<AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel entity);
        Task<EntityOperationResult<TExtended>> ReadAsync<TExtended>(ClaimsPrincipal principal, string id, string orgId = null) where TExtended : AmphoraModel;
        Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity);
    }
}