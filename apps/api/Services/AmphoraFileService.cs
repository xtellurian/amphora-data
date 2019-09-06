using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services
{
    public class AmphoraFileService : IAmphoraFileService
    {
        private readonly IPermissionService permissionService;
        private readonly IDataStore<Common.Models.Amphora, byte[]> store;
        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraFileService> logger;

        public AmphoraFileService(
            ILogger<AmphoraFileService> logger,
            IPermissionService permissionService,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> store,
            IUserManager userManager)
        {
            this.permissionService = permissionService;
            this.store = store;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, Amphora.Common.Models.Amphora entity, string file)
        {
            var user = await userManager.GetUserAsync(principal);
            var granted = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.ReadContents);

            if (granted)
            {
                var data = await this.store.GetDataAsync(entity, file);
                return new EntityOperationResult<byte[]>(data);
            }
            else
            {
                logger.LogInformation($"Permission denied to user {user.Id} to read contents of {entity.Id}");
                return new EntityOperationResult<byte[]>("Permission Denied") { WasForbidden = true };
            }
        }
        public async Task<EntityOperationResult<byte[]>> WriteFileAsync(
            ClaimsPrincipal principal,
            Amphora.Common.Models.Amphora entity,
            byte[] contents,
            string file)
        {
            var user = await userManager.GetUserAsync(principal);
            var granted = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.WriteContents);

            if (granted)
            {
                var data = await this.store.SetDataAsync(entity, contents, file);
                return new EntityOperationResult<byte[]>(data);
            }
            else
            {
                logger.LogInformation($"Permission denied to user {user.Id} to write contents of {entity.Id}");
                return new EntityOperationResult<byte[]>("Permission Denied") { WasForbidden = true };
            }
        }

    }
}