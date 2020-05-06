using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface ITermsOfUseService
    {
        Task<EntityOperationResult<TermsOfUseAcceptanceModel>> AcceptAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<TermsOfUseModel>> CreateAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<TermsOfUseModel>> CreateGlobalAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<TermsOfUseModel>> DeleteAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<IEnumerable<TermsOfUseModel>>> ListAsync(ClaimsPrincipal principal);
        Task<EntityOperationResult<TermsOfUseModel>> ReadAsync(ClaimsPrincipal principal, string touId);
        Task<EntityOperationResult<TermsOfUseModel>> UpdateAsync(ClaimsPrincipal principal, TermsOfUseModel model);
    }
}