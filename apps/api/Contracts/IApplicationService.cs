using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Applications;

namespace Amphora.Api.Contracts
{
    public interface IApplicationService
    {
        Task<EntityOperationResult<IEnumerable<ApplicationModel>>> ListAsync(ClaimsPrincipal principal);
        Task<EntityOperationResult<ApplicationModel>> CreateAsync(ClaimsPrincipal principal, ApplicationModel model);
        Task<EntityOperationResult<ApplicationModel>> DeleteAsync(ClaimsPrincipal principal, string id);
        Task<EntityOperationResult<ApplicationModel>> ReadAsync(ClaimsPrincipal principal, string id);
        Task<EntityOperationResult<ApplicationModel>> UpdateAsync(ClaimsPrincipal principal, ApplicationModel model);
    }
}