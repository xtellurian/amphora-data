using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Exceptions;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Infrastructure.Models;
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
            this.client = factory.CreateClient(HttpClientNames.IdentityServerClient);
            client.BaseAddress = options.CurrentValue.IdentityUri();
            this.options = options;
            this.logger = logger;
        }

        public async Task<(bool success, string token)> GetToken(LoginRequest request)
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

        public async Task<EntityOperationResult<AmphoraUser>> CreateUser(CreateAmphoraUser user, string password)
        {
            var u = new CreateAmphoraUser
            {
                Email = user.Email,
                About = user.About,
                UserName = user.UserName,
                FullName = user.FullName,
                Password = password,
                ConfirmPassword = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(u), Encoding.UTF8, "application/json");
            try
            {
                var response = await this.client.PostAsync("/api/users", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var responseUser = JsonConvert.DeserializeObject<AmphoraUser>(await response.Content.ReadAsStringAsync());
                    // so now query the application user
                    return new EntityOperationResult<AmphoraUser>(responseUser, responseUser);
                }
                else
                {
                    return new EntityOperationResult<AmphoraUser>(false);
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                logger.LogCritical(httpRequestException.Message);
                throw new IdentityServerException("The Identity Server did not respond as expected", httpRequestException);
            }
        }

        public async Task<EntityOperationResult<AmphoraUser>> DeleteUser(ClaimsPrincipal principal, IUser user)
        {
            if (principal.GetUserId() == user.Id)
            {
                var deleteRes = await client.DeleteAsync($"/api/users?userName={user.UserName}");
                if (deleteRes.IsSuccessStatusCode)
                {
                    return new EntityOperationResult<AmphoraUser>(true);
                }
                else
                {
                    return new EntityOperationResult<AmphoraUser>(await deleteRes.Content.ReadAsStringAsync());
                }
            }
            else
            {
                return new EntityOperationResult<AmphoraUser>("Can only delete user yourself");
            }
        }
    }
}