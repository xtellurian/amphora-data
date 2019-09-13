using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(IApplicationUser user, Common.Contracts.IEntity entity, string resourcePermission);
        Task<PermissionModel> SetIsOwner(IApplicationUser user, Common.Contracts.IEntity entity);
    }
}