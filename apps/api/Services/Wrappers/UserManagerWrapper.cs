using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Wrappers
{
    public class UserManagerWrapper<T> : IUserManager where T : class, IApplicationUser
    {
        private readonly UserManager<T> userManager;
        private readonly IMapper mapper;

        public UserManagerWrapper(UserManager<T> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IdentityResult> CreateAsync(IApplicationUser user, string password)
        {
            var mapped = mapper.Map<T>(user);
            return await userManager.CreateAsync(mapped, password);
        }

        public async Task<IdentityResult> DeleteAsync(IApplicationUser user)
        {
            var mapped = mapper.Map<T>(user);
            return await userManager.DeleteAsync(mapped);
        }

        public async Task<IApplicationUser> FindByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(IApplicationUser user)
        {
            var mapped = mapper.Map<T>(user);
            return await userManager.GenerateEmailConfirmationTokenAsync(mapped);
        }

        public string GetUserName(ClaimsPrincipal principal)
        {
            return userManager.GetUserName(principal);
        }

        public async Task<IApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return await userManager.GetUserAsync(principal);
        }

        public async Task<IdentityResult> UpdateAsync(IApplicationUser user)
        {
            return await userManager.UpdateAsync((T)user);
        }
    }
}