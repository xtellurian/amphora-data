using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Users;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface ISignInManager
    {
        Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user);
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        bool IsSignedIn(ClaimsPrincipal principal);
        Task<SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null);
        Task SignOutAsync();
    }
}   