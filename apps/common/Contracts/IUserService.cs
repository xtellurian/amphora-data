using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Contracts
{
    public interface IUserService // this is for CRUD ops to apply permissions
    {
        IEntityStore<ApplicationUser> UserStore { get; }
        Task<ApplicationUser> ReadUserModelAsync(ClaimsPrincipal principal);
        Task<EntityOperationResult<ApplicationUser>> CreateAsync(ApplicationUser user, InvitationModel invitation, string password);
        Task<EntityOperationResult<ApplicationUser>> UpdateAsync(ClaimsPrincipal principal, ApplicationUser user);
        Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user);
        bool IsSignedIn(ClaimsPrincipal principal);
    }
}