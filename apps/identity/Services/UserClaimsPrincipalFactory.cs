using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Users;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Services
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>, IUserClaimsPrincipalFactory<ApplicationUser>
    {
        public UserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("full_name", user.FullName ?? ""));
            identity.AddClaim(new Claim("about", user.About ?? ""));
            identity.AddClaim(new Claim("email", user.Email ?? ""));
            identity.AddClaim(new Claim("email_confirmed", user.EmailConfirmed.ToString()));
            identity.AddClaim(new Claim("global_admin",
                (user.EmailConfirmed && (user.Email?.ToLower().EndsWith("@amphoradata.com") ?? false)).ToString()));
            return identity;
        }
    }
}