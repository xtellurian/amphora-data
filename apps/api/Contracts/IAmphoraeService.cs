using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraeService
    {
        IEntityStore<AmphoraModel> AmphoraStore { get; }

        Task<IQueryable<AmphoraModel>> AmphoraPurchasedBy(ClaimsPrincipal principal, IUser user);
        Task<EntityOperationResult<AmphoraModel>> CreateAsync(ClaimsPrincipal principal, AmphoraModel model);
        Task<EntityOperationResult<AmphoraModel>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel entity);
        Task<EntityOperationResult<IEnumerable<AmphoraModel>>> ListForOrganisationAsync(ClaimsPrincipal principal,
                                                                                        int skip,
                                                                                        int take,
                                                                                        bool created = true,
                                                                                        bool purchased = false);
        Task<EntityOperationResult<IEnumerable<AmphoraModel>>> ListForSelfAsync(ClaimsPrincipal principal,
                                                                                int skip,
                                                                                int take,
                                                                                bool created = true,
                                                                                bool purchased = false);
        Task<EntityOperationResult<AmphoraModel>> ReadAsync(ClaimsPrincipal principal, string id, bool includeChildren = false, string orgId = null);
        Task<EntityOperationResult<AmphoraModel>> UpdateAsync(ClaimsPrincipal principal, AmphoraModel entity);
    }
}