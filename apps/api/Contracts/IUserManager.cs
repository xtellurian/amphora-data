using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface IUserManager
    {
        Task<IdentityResult> CreateAsync(IApplicationUser user, string password);
        Task<IdentityResult> DeleteAsync(IApplicationUser user);
        Task<IApplicationUser> FindByNameAsync(string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(IApplicationUser user);
        Task<IApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        Task<IdentityResult> UpdateAsync(IApplicationUser user);
    }
}