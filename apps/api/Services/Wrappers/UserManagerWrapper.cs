using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Amphora.Api.Services.Wrappers
{
    public class UserManagerWrapper<T> : IUserManager where T : ApplicationUser
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UserManagerWrapper(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var mapped = mapper.Map<T>(user);
            var u = await userManager.FindByIdAsync(user.Id);
            return await userManager.DeleteAsync(u);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var mapped = mapper.Map<T>(user);
            return await userManager.GenerateEmailConfirmationTokenAsync(mapped);
        }

        public string GetUserName(ClaimsPrincipal principal)
        {
            return userManager.GetUserName(principal);
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return await userManager.GetUserAsync(principal);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return await userManager.UpdateAsync(user);
        }
    }
}