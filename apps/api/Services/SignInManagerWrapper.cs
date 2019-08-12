using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Development;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class SignInManagerWrapper<T> : ISignInManager<T> where T: IdentityUserV2
    {
        private readonly SignInManager<T> signInManager;
        private static bool isSignedIn = false;

        public SignInManagerWrapper()
        {
            
        }
        public SignInManagerWrapper(SignInManager<T> signInManager)
        {
            this.signInManager = signInManager;
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            if (signInManager == null) return isSignedIn;
            return signInManager.IsSignedIn(principal);
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            if(signInManager == null) return new List<AuthenticationScheme>();
            return await signInManager.GetExternalAuthenticationSchemesAsync() ;
        }

        public async Task<SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure )
        {
            if(signInManager == null) {
                isSignedIn = true;
                return new DevSignInResult(true);
            }
            return await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignInAsync(T user, bool isPersistent, string authenticationMethod = null)
        {
            if (signInManager == null) return;
            await signInManager.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public async Task SignOutAsync()
        {
            if(signInManager == null) 
            {
                isSignedIn = false;
                return;
            };
            await signInManager.SignOutAsync();
        }
    }
}