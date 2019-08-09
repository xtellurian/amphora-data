using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class UserManagerWrapper<T> : IUserManager<T> where T : IdentityUserV2, new()
    {
        private readonly UserManager<T> userManager;

        public UserManagerWrapper()
        {

        }
        public UserManagerWrapper(UserManager<T> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(T user, string password)
        {
            if (userManager == null) return new IdentityResult();
            return await userManager.CreateAsync(user, password);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(T user)
        {
            if (userManager == null) return "_blank";
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public string GetUserName(ClaimsPrincipal principal)
        {
            return userManager?.GetUserName(principal) ?? "Default User";
        }

        public async Task<T> GetUserAsync(ClaimsPrincipal principal)
        {
            if (userManager == null) return new T();
            return await userManager.GetUserAsync(principal);
        }
    }
}