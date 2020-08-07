using System.Collections.Generic;
using System.Security.Claims;
using Amphora.Common.Models.Platform;

namespace Amphora.Identity.Contracts
{
    public interface IAmphoraClaimsService
    {
        IEnumerable<Claim> AllClaims(ClaimsPrincipal principal, LoginRequest loginRequest);
    }
}