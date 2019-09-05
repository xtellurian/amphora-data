using System.Threading.Tasks;
using Amphora.Api.Models;

namespace Amphora.Api.Contracts
{
    public interface IUserService // this is for CRUD ops to apply permissions
    {
        Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user, string password);
        Task<EntityOperationResult<IApplicationUser>> DeleteAsync(IApplicationUser user);
    }
}