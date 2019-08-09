using System.Security.Claims;
using System.Threading.Tasks;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface IUserManager<T> where T : IdentityUserV2
    {
        Task<IdentityResult> CreateAsync(T user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(T user);
        Task<T> GetUserAsync(ClaimsPrincipal principal);
        string GetUserName(ClaimsPrincipal principal);
        
    }
}