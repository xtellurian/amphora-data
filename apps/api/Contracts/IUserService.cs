using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IUserService // this is for CRUD ops to apply permissions
    {
        Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user,
                                                                  string password,
                                                                  RoleAssignment.Roles role = RoleAssignment.Roles.User);
        Task<EntityOperationResult<IApplicationUser>> DeleteAsync(IApplicationUser user);
    }
}