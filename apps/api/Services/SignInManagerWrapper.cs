using System.Security.Claims;
using Amphora.Api.Contracts;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class SignInManagerWrapper<T> : ISignInManager<T> where T: IdentityUserV2
    {
        private readonly SignInManager<T> signInManager;

        public SignInManagerWrapper()
        {
            
        }
        public SignInManagerWrapper(SignInManager<T> signInManager)
        {
            this.signInManager = signInManager;
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return signInManager?.IsSignedIn(principal) ?? true;
        }
    }
}