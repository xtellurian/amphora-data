using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Services.Wrappers
{
    public class IdServerUserManager : IUserManager
    {
        private readonly IEntityStore<ApplicationUser> userStore;
        private readonly ILogger<IdServerUserManager> logger;
        private readonly HttpClient client;

        public IdServerUserManager(IEntityStore<ApplicationUser> userStore,
                                   IHttpClientFactory factory,
                                   ILogger<IdServerUserManager> logger,
                                   IOptionsMonitor<ExternalServices> services)
        {
            this.userStore = userStore;
            this.logger = logger;
            this.client = factory.CreateClient("idServer");
            logger.LogInformation($"Identity Server Base URL is {services.CurrentValue?.IdentityBaseUrl}");
            this.client.BaseAddress = new System.Uri(services.CurrentValue.IdentityBaseUrl);
        }

        public Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            throw new System.NotImplementedException();
        }

        public async Task<(IdentityResult idResult, AmphoraUser user)> CreateAsync(ApplicationUser user, string password)
        {
            var u = new CreateAmphoraUser
            {
                Email = user.Email,
                About = user.About,
                UserName = user.UserName,
                FullName = user.FullName,
                Password = password,
            };
            var content = new StringContent(JsonConvert.SerializeObject(u), Encoding.UTF8, "application/json");
            var response = await this.client.PostAsync("/api/users", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var createdUser = JsonConvert.DeserializeObject<AmphoraUser>(responseContent);
                return (IdentityResult.Success, createdUser);
            }
            else
            {
                return (IdentityResult.Failed(new IdentityError { Description = responseContent }), null);
            }
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await userStore.ReadAsync(userId);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var users = await userStore.QueryAsync(_ => _.UserName == userName);
            return users.FirstOrDefault();
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            if (userId == null)
            {
                return null;
            }
            else
            {
                return await FindByIdAsync(userId);
            }
        }

        public string GetUserName(ClaimsPrincipal principal)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}