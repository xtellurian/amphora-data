using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Wrappers
{
    public class IdServerSignInManager : ISignInManager
    {
        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            throw new System.NotImplementedException();
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            throw new System.NotImplementedException();
        }

        public Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            throw new System.NotImplementedException();
        }

        public Task SignOutAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}