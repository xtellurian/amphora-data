using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface IUserService // this is for CRUD ops to apply permissions
    {
        IUserManager UserManager { get; }
        IEntityStore<ApplicationUser> UserStore { get; }

        Task<EntityOperationResult<ApplicationUser>> CreateAsync(ApplicationUser user, InvitationModel invitation, string password);
        Task<EntityOperationResult<ApplicationUser>> DeleteAsync(ClaimsPrincipal principal, IUser user);
        bool IsSignedIn(ClaimsPrincipal principal);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task<ApplicationUser> ReadUserModelAsync(ClaimsPrincipal principal);
    }
}