using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;

namespace Amphora.Common.Contracts
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(IUser user, AmphoraModel amphora, AccessLevels accessLevel);
        Task<bool> IsAuthorizedAsync(IUser user, OrganisationModel? organisation, AccessLevels accessLevel);
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal principal, AmphoraModel entity, AccessLevels accessLevel);
    }
}