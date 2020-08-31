using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using Amphora.Identity.Contracts;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Services
{
    public class AmphoraClaimsService : IAmphoraClaimsService
    {
        private readonly ILogger<AmphoraClaimsService> logger;

        public AmphoraClaimsService(ILogger<AmphoraClaimsService> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Claim> AllClaims(ClaimsPrincipal principal, LoginRequest loginRequest)
        {
            var result = new List<Claim>();
            result.AddRange(principal.Claims);
            if (loginRequest.Claims != null && loginRequest.Claims.Any())
            {
                foreach (var claim in loginRequest.Claims)
                {
                    if (IsValidClaim(claim))
                    {
                        result.Add(new Claim(claim.Type, claim.Value));
                    }
                    else
                    {
                        logger.LogWarning($"The claim type {claim.Type} with value {claim.Value} was invalid.");
                    }
                }
            }

            return result;
        }

        private bool IsValidClaim(LoginClaim c)
        {
            if (!string.Equals(c.Type, "scope"))
            {
                return false;
            }
            else
            {
                return c.Value == Claims.Purchase;
            }
        }
    }
}