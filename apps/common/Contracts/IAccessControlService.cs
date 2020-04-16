using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions.Rules;

namespace Amphora.Common.Contracts
{
    public interface IAccessControlService
    {
        Task<EntityOperationResult<AccessRule>> CreateAsync(ClaimsPrincipal principal, AmphoraModel amphora, AccessRule rule);
        Task<EntityOperationResult<AccessRule>> DeleteAsync(ClaimsPrincipal principal, AmphoraModel amphora, string ruleId);
    }
}