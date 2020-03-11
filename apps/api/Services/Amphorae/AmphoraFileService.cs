using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraFileService : IAmphoraFileService
    {
        private readonly IPermissionService permissionService;

        public IBlobStore<AmphoraModel> Store { get; }

        private readonly IUserManager userManager;
        private readonly ILogger<AmphoraFileService> logger;

        public AmphoraFileService(
            ILogger<AmphoraFileService> logger,
            IPermissionService permissionService,
            IBlobStore<AmphoraModel> store,
            IUserManager userManager)
        {
            this.permissionService = permissionService;
            this.Store = store;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file)
        {
            var user = await userManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(user)))
            {
                var granted = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.ReadContents);

                if (granted)
                {
                    var data = await this.Store.ReadBytesAsync(entity, file);
                    return new EntityOperationResult<byte[]>(user, data);
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {user.Id} to read contents of {entity.Id}");
                    return new EntityOperationResult<byte[]>(user, "Permission Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<UploadResponse>> CreateFileAsync(
            ClaimsPrincipal principal,
            AmphoraModel entity,
            string file)
        {
            var user = await userManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(user)))
            {
                var granted = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.WriteContents);

                if (granted)
                {
                    if (await Store.ExistsAsync(entity, file))
                    {
                        // file already exists. Return error.
                        return new EntityOperationResult<UploadResponse>(user, 409, $"{file} already exists. Delete the file and upload again.");
                    }

                    var url = await this.Store.GetWritableUrl(entity, file);
                    return new EntityOperationResult<UploadResponse>(user, new UploadResponse(url));
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {user.Id} to write contents of {entity.Id}");
                    return new EntityOperationResult<UploadResponse>(user, "Permission Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<UploadResponse>> WriteFileAsync(
            ClaimsPrincipal principal,
            AmphoraModel entity,
            byte[] contents,
            string file)
        {
            var user = await userManager.GetUserAsync(principal);
            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(user)))
            {
                var granted = await permissionService.IsAuthorizedAsync(user, entity, ResourcePermissions.WriteContents);

                if (granted)
                {
                    if (await Store.ExistsAsync(entity, file))
                    {
                        // file already exists. Return error.
                        return new EntityOperationResult<UploadResponse>(user, 409, $"{file} already exists. Delete the file and upload again.");
                    }

                    if (contents.Length > 0)
                    {
                        logger.LogInformation("Writing contents");
                        await this.Store.WriteBytesAsync(entity, file, contents);
                    }
                    else
                    {
                        logger.LogInformation("Contents are empty");
                    }

                    var url = await this.Store.GetWritableUrl(entity, file);
                    return new EntityOperationResult<UploadResponse>(user, new UploadResponse(url));
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {user.Id} to write contents of {entity.Id}");
                    return new EntityOperationResult<UploadResponse>(user, "Permission Denied") { WasForbidden = true };
                }
            }
        }
    }
}