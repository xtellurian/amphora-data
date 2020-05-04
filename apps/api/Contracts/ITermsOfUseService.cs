using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface ITermsOfUseService
    {
        Task<EntityOperationResult<TermsOfUseModel>> CreateAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<TermsOfUseModel>> DeleteAsync(ClaimsPrincipal principal, TermsOfUseModel model);
        Task<EntityOperationResult<TermsOfUseModel>> ReadAsync(ClaimsPrincipal principal, string touId);
        Task<EntityOperationResult<TermsOfUseModel>> UpdateAsync(ClaimsPrincipal principal, TermsOfUseModel model);
    }
}