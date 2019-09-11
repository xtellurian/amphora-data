using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<PermissionModel> CreateOrganisationalRole(IApplicationUser user, RoleAssignment.Roles role, OrganisationModel org);
        Task<bool> IsAuthorizedAsync(IApplicationUser user, Common.Contracts.IEntity entity, string resourcePermission);
        Task<PermissionModel> SetIsOwner(IApplicationUser user, Common.Contracts.IEntity entity);
    }
}