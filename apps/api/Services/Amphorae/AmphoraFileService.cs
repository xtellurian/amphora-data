using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae.Files;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Logging;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class AmphoraFileService : IAmphoraFileService
    {
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;

        public IBlobStore<AmphoraModel> Store { get; }

        private readonly ILogger<AmphoraFileService> logger;
        private readonly IPlanLimitService planLimitService;

        public AmphoraFileService(
            ILogger<AmphoraFileService> logger,
            IPlanLimitService planLimitService,
            IPermissionService permissionService,
            IBlobStore<AmphoraModel> store,
            IUserDataService userDataService)
        {
            this.permissionService = permissionService;
            this.Store = store;
            this.userDataService = userDataService;
            this.logger = logger;
            this.planLimitService = planLimitService;
        }

        private async Task<EntityOperationResult<AmphoraFileSize>> GetSizeAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<AmphoraFileSize>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(userReadRes.Entity)))
            {
                var granted = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.ReadContents);

                if (granted)
                {
                    var size = await Store.GetContainerSizeAsync(entity);
                    return new EntityOperationResult<AmphoraFileSize>(userReadRes.Entity, new AmphoraFileSize(size));
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {userReadRes.Entity.Id} to read contents of {entity.Id}");
                    return new EntityOperationResult<AmphoraFileSize>(userReadRes.Entity, "Permission Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<byte[]>> ReadFileAsync(ClaimsPrincipal principal, AmphoraModel entity, string file)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<byte[]>("Unknown User");
            }

            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(userReadRes.Entity)))
            {
                var granted = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.ReadContents);

                if (granted)
                {
                    var data = await this.Store.ReadBytesAsync(entity, file);
                    return new EntityOperationResult<byte[]>(userReadRes.Entity, data);
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {userReadRes.Entity.Id} to read contents of {entity.Id}");
                    return new EntityOperationResult<byte[]>(userReadRes.Entity, "Permission Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<UploadResponse>> CreateFileAsync(
            ClaimsPrincipal principal,
            AmphoraModel entity,
            string file)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<UploadResponse>(userReadRes.Message);
            }

            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(userReadRes.Entity)))
            {
                var granted = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.WriteContents);

                if (granted)
                {
                    if (await Store.ExistsAsync(entity, file))
                    {
                        // file already exists. Return error.
                        return new EntityOperationResult<UploadResponse>(userReadRes.Entity, $"{file} already exists. Delete the file and upload again.");
                    }

                    var url = await this.Store.GetWritableUrl(entity, file);
                    return new EntityOperationResult<UploadResponse>(userReadRes.Entity, new UploadResponse(url));
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {userReadRes.Entity.Id} to write contents of {entity.Id}");
                    return new EntityOperationResult<UploadResponse>(userReadRes.Entity, "Permission Denied") { WasForbidden = true };
                }
            }
        }

        public async Task<EntityOperationResult<UploadResponse>> WriteFileAsync(
            ClaimsPrincipal principal,
            AmphoraModel entity,
            byte[] contents,
            string file)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (!userReadRes.Succeeded)
            {
                return new EntityOperationResult<UploadResponse>(userReadRes.Message);
            }

            using (logger.BeginScope(new LoggerScope<AmphoraFileService>(principal)))
            {
                var granted = await permissionService.IsAuthorizedAsync(userReadRes.Entity, entity, ResourcePermissions.WriteContents);

                if (granted)
                {
                    if (await Store.ExistsAsync(entity, file))
                    {
                        // file already exists. Return error.
                        return new EntityOperationResult<UploadResponse>(
                            userReadRes.Entity,
                            $"{file} already exists. Delete the file and upload again.")
                        { Code = 409 };
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
                    return new EntityOperationResult<UploadResponse>(userReadRes.Entity, new UploadResponse(url));
                }
                else
                {
                    logger.LogInformation($"Permission denied to user {userReadRes.Entity.Id} to write contents of {entity.Id}");
                    return new EntityOperationResult<UploadResponse>(userReadRes.Entity, "Permission Denied") { WasForbidden = true };
                }
            }
        }
    }
}