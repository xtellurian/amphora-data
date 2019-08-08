using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace Amphora.Api.Contracts
{
    public interface ISignInManager<T> where T : IdentityUserV2
    {
        bool IsSignedIn(System.Security.Claims.ClaimsPrincipal principal);
    }
}