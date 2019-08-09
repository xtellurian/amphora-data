using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface ISignInManager<T> where T : IdentityUserV2
    {
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        bool IsSignedIn(ClaimsPrincipal principal);
        Task<SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignInAsync(T user, bool isPersistent, string authenticationMethod = null);
        Task SignOutAsync();
    }
}   