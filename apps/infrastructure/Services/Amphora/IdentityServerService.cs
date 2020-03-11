using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Infrastructure.Services
{
    public class IdentityServerService : IAuthenticateService, IIdentityService
    {
        private readonly IOptionsMonitor<ExternalServices> options;
        private readonly ILogger<IdentityServerService> logger;
        private HttpClient client;

        public IdentityServerService(IHttpClientFactory factory,
                                     IOptionsMonitor<ExternalServices> options,
                                     ILogger<IdentityServerService> logger)
        {
            this.client = factory.CreateClient("identityServer");
            client.BaseAddress = new System.Uri(options.CurrentValue.IdentityBaseUrl);
            this.options = options;
            this.logger = logger;
        }

        public async Task<(bool success, string token)> GetToken(Common.Models.Platform.TokenRequest request)
        {
            logger.LogInformation($"Getting token for {request.Username}");
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Failed to get token from ID Server, ${responseContent}");
                return (false, "Failed to get a token from Id Server");
            }
            else
            {
                return (response.IsSuccessStatusCode, responseContent);
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> CreateUser(ClaimsPrincipal principal, ApplicationUser user, string password)
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
                var responseUser = JsonConvert.DeserializeObject<AmphoraUser>(await response.Content.ReadAsStringAsync());
                // so now query the application user
                user.Id = responseUser.Id;
                return new EntityOperationResult<ApplicationUser>(user, user);
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>(false);
            }
        }

        public async Task<EntityOperationResult<ApplicationUser>> DeleteUser(ClaimsPrincipal principal, IUser user)
        {
            if (principal.GetUserId() == user.Id)
            {
                var deleteRes = await client.DeleteAsync($"/api/users?userName={user.UserName}");
                if (deleteRes.IsSuccessStatusCode)
                {
                    return new EntityOperationResult<ApplicationUser>(true);
                }
                else
                {
                    return new EntityOperationResult<ApplicationUser>(await deleteRes.Content.ReadAsStringAsync());
                }
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>("Can only delete user yourself");
            }
        }
    }
}