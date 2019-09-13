using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Wrappers
{
    public class SignInManagerWrapper<T> : ISignInManager where T : class, IApplicationUser
    {
        private readonly SignInManager<T> signInManager;
        private readonly IMapper mapper;

        public SignInManagerWrapper(SignInManager<T> signInManager, IMapper mapper)
        {
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return signInManager.IsSignedIn(principal);
        }

        public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(IApplicationUser user)
        {
            var mapped = mapper.Map<T>(user);
            return await this.signInManager.CreateUserPrincipalAsync(mapped);
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return await signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(string user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignInAsync(IApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            var mapped = mapper.Map<T>(user);
            await signInManager.SignInAsync(mapped, isPersistent, authenticationMethod);
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}