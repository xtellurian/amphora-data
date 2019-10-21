using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;

namespace Amphora.Api.Contracts
{
    public interface IPurchaseService
    {
        Task<bool> HasAgreedToTermsAndConditionsAsync(ClaimsPrincipal principal, AmphoraModel amphora);
        Task<EntityOperationResult<PurchaseModel>> PurchaseAmphora(ClaimsPrincipal principal, AmphoraModel amphora);
    }
}