using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface IUserManager<T> where T : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser
    {
        Task<IdentityResult> CreateAsync(T user, string password);
        Task<T> FindByNameAsync(string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(T user);
        Task<T> GetUserAsync(ClaimsPrincipal principal);
        string GetUserName(ClaimsPrincipal principal);
        Task<IdentityResult> UpdateAsync(T user);
    }
}