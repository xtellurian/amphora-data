using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorized(Models.IApplicationUser user, Common.Contracts.IEntity entity, string resourcePermission);
        Task<Common.Models.PermissionCollection> SetIsOwner(Models.IApplicationUser user, Common.Contracts.IEntity entity);
    }
}