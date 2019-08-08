using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace Amphora.Api.Contracts
{
    public interface IUserManager<T> where T : IdentityUserV2
    {
        System.Threading.Tasks.Task<T> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal);
        string GetUserName(System.Security.Claims.ClaimsPrincipal principal);
        
    }
}