using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Amphorae
{
    public class TermsOfUseService : ITermsOfUseService
    {
        public Task<EntityOperationResult<TermsOfUseModel>> CreateAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            throw new System.NotImplementedException();
        }

        public Task<EntityOperationResult<TermsOfUseModel>> ReadAsync(ClaimsPrincipal principal, string touId)
        {
            throw new System.NotImplementedException();
        }

        public Task<EntityOperationResult<TermsOfUseModel>> UpdateAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            throw new System.NotImplementedException();
        }

        public Task<EntityOperationResult<TermsOfUseModel>> DeleteAsync(ClaimsPrincipal principal, TermsOfUseModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}