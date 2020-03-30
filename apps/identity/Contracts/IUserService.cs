using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Platform;
using Amphora.Identity.Models;

namespace Amphora.Identity.Contracts
{
    public interface IUserService
    {
        Task<ApplicationUser> ReadUserModelAsync(ClaimsPrincipal principal);
        Task<EntityOperationResult<ApplicationUser>> CreateAsync(ApplicationUser user, InvitationModel invitation, string password);
        Task<EntityOperationResult<ApplicationUser>> UpdateAsync(ClaimsPrincipal principal, ApplicationUser user);
        Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user);
    }
}