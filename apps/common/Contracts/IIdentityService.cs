using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Contracts
{
    public interface IIdentityService
    {
        Task<EntityOperationResult<ApplicationUser>> CreateUser(ApplicationUser user, string password);
        Task<EntityOperationResult<ApplicationUser>> DeleteUser(ClaimsPrincipal principal, IUser user);
    }
}