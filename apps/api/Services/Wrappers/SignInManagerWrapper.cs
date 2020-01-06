using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Wrappers
{
    public class SignInManagerWrapper<T> : ISignInManager where T : ApplicationUser
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

        public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
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

        public async Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            if (user is T)
            {
                await signInManager.SignInAsync(user as T, isPersistent, authenticationMethod);
            }
            else
            {
                var mapped = mapper.Map<T>(user);
                await signInManager.SignInAsync(mapped, isPersistent, authenticationMethod);
            }
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}