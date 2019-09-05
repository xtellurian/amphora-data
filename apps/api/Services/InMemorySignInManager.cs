using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class InMemorySignInManager : ISignInManager
    {
        public InMemorySignInManager(IUserManager userManager)
        {
            this.userManager = userManager;
        }
        private Dictionary<string, ClaimsPrincipal> UserPrincipals = new Dictionary<string, ClaimsPrincipal>();
        private readonly IUserManager userManager;

        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(IApplicationUser user)
        {
            return Task<ClaimsPrincipal>.Factory.StartNew(() =>
            {
                if (UserPrincipals.ContainsKey(user.Id))
                {
                    return UserPrincipals[user.Id];
                }
                else
                {
                    ClaimsIdentity identity = GetClaimsIdentity(user);
                    return new ClaimsPrincipal(new List<ClaimsIdentity>{
                         identity
                    });
                }
            });
        }

        private static ClaimsIdentity GetClaimsIdentity(IApplicationUser user)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            return identity;
        }

        public Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return Task<IEnumerable<AuthenticationScheme>>.Factory.StartNew(() =>
            {
                return new List<AuthenticationScheme>();
            });
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return UserPrincipals.Values.Any(); // always sign in a this time

        }

        public async Task<SignInResult> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            else
            {
                var principal = await CreateUserPrincipalAsync( (TestApplicationUser) user);
                this.UserPrincipals.Add(user.Id, principal);
                return SignInResult.Success;
            }
        }

        public async Task SignInAsync(IApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            var identity = await CreateUserPrincipalAsync(user);
            this.UserPrincipals.Add(user.Id, identity);

            // await HttpContext.SignInAsync(
            //     CookieAuthenticationDefaults.AuthenticationScheme,
            //     new ClaimsPrincipal(claimsIdentity),
            //     authProperties);
        }

        public Task SignOutAsync()
        {
            UserPrincipals.Clear();
            return Task.CompletedTask;
        }
    }
}