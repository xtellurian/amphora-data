using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Security;
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
            identity.AddClaim(new Claim(Claims.About, user.About ?? ""));
            identity.AddClaim(new Claim(Claims.Email, user.Email ?? ""));
            identity.AddClaim(new Claim(Claims.EmailConfirmed, user.EmailConfirmed.ToString()));
            identity.AddClaim(new Claim(Claims.FullName, user.FullName ?? ""));
            if (user.EmailConfirmed && (user.Email?.ToLower().EndsWith("@amphoradata.com") ?? false))
            {
                identity.AddClaim(new Claim(Claims.GlobalAdmin, true.ToString()));
            }

            return identity;
        }
    }
}