using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services
{
    public class UserManagerWrapper<T>: IUserManager<T> where T: IdentityUserV2, new()
    {
        private readonly UserManager<T> userManager;

        public UserManagerWrapper()
        {

        }
        public UserManagerWrapper(UserManager<T> userManager)
        {
            this.userManager = userManager;
        }

        public string GetUserName(ClaimsPrincipal principal)
        {
            return userManager?.GetUserName(principal) ?? "Default User";
        }

        public async Task<T> GetUserAsync(ClaimsPrincipal principal)
        {
            if(userManager == null) return new T();
            return await userManager.GetUserAsync(principal);
        }
    }
}