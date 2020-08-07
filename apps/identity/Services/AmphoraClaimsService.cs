using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using Amphora.Identity.Contracts;

namespace Amphora.Identity.Services
{
    public class AmphoraClaimsService : IAmphoraClaimsService
    {
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
                }
            }

            return result;
        }

        private bool IsValidClaim(LoginClaim c)
        {
            return c.Type == Claims.Purchase;
        }
    }
}