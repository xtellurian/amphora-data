using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class UserManagerWrapper<T> : IUserManager<T> where T : Microsoft.AspNetCore.Identity.DocumentDB.IdentityUser, new()
    {
        private readonly UserManager<T> userManager;
        private readonly ApplicationUser dev = new ApplicationUser
        {
            UserName = "Developer",
            OrganisationId = "Developer"
        };

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

        public async Task<T> FindByNameAsync(string userName)
        {
            if (userManager == null) return dev as T;
            return await userManager.FindByNameAsync(userName);
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
            if (userManager == null) return dev as T;
            return await userManager.GetUserAsync(principal);
        }

        public async Task<IdentityResult> UpdateAsync(T user)
        {
            if(userManager==null) return new IdentityResult();
            return await userManager.UpdateAsync(user);
        }
    }
}