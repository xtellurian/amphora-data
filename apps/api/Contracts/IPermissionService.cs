using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<PermissionCollection> CreateOrganisationalRole(IApplicationUser user, RoleAssignment.Roles role, Organisation org);
        Task<bool> IsAuthorized(IApplicationUser user, Common.Contracts.IEntity entity, string resourcePermission);
        Task<PermissionCollection> SetIsOwner(IApplicationUser user, Common.Contracts.IEntity entity);
    }
}