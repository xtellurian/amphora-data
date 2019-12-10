using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Contracts
{
    public interface IUserManager
    {
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        string GetUserName(ClaimsPrincipal principal);
        Task<bool> HasPasswordAsync(ApplicationUser user);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}