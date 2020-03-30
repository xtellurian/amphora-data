using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(IUser user, AmphoraModel amphora, AccessLevels accessLevel);
        Task<bool> IsAuthorizedAsync(IUser user, Common.Models.Organisations.OrganisationModel organisation, AccessLevels accessLevel);
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal principal, AmphoraModel entity, AccessLevels accessLevel);
    }
}