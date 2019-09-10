using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        IEntityStore<Common.Models.Amphora> AmphoraStore { get; }

        Task<EntityOperationResult<Common.Models.Amphora>> CreateAsync(ClaimsPrincipal principal, Common.Models.Amphora model);
        Task<EntityOperationResult<Common.Models.Amphora>> DeleteAsync(ClaimsPrincipal principal, Common.Models.Amphora entity);
        Task<EntityOperationResult<Common.Models.Amphora>> ReadAsync(ClaimsPrincipal principal, string id, string orgId = null);
        Task<EntityOperationResult<Common.Models.Amphora>> UpdateAsync(ClaimsPrincipal principal, Common.Models.Amphora entity);
    }
}