using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Dtos.Users;

namespace Amphora.Common.Contracts
{
    public interface IIdentityService
    {
        Task<EntityOperationResult<AmphoraUser>> CreateUser(CreateAmphoraUser user, string password);
        Task<EntityOperationResult<AmphoraUser>> DeleteUser(ClaimsPrincipal principal, IUser user);
    }
}