using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Security;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Services
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>, IUserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly ILogger<UserClaimsPrincipalFactory> logger;

        public UserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            ILogger<UserClaimsPrincipalFactory> logger,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            this.logger = logger;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            logger.LogInformation($"Generating Claims for User({user.Id})");
            var identity = await base.GenerateClaimsAsync(user);
            if (!identity.Claims.Any(_ => _.Type == Claims.Name))
            {
                logger.LogInformation($"Adding username claim ({user.UserName}) for User({user.Id})");
                identity.AddClaim(new Claim(Claims.Name, user.UserName));
            }

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