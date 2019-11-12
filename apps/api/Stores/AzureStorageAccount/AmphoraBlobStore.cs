using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Models.Amphorae;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class AmphoraBlobStore : AzBlobBase, IBlobStore<AmphoraModel>
    {
        public AmphoraBlobStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AmphoraBlobStore> logger) : base(options, logger)
        {
        }

        public async Task<byte[]> ReadBytesAsync(AmphoraModel entity, string path)
        {
            var container = GetContainerReference(entity);
            if (!await container.ExistsAsync())
            {
                return null; // empty
            }

            var blob = container.GetBlockBlobReference(path);
            var buffer = new MemoryStream();
            await blob.DownloadToStreamAsync(buffer);
            return buffer.ToArray();
        }

        public async Task WriteBytesAsync(AmphoraModel entity, string path, byte[] bytes)
        {
            // 1 container per amphora
            var container = GetContainerReference(entity);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {entity.Id}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {entity.Id}. ${blob.Uri}");
            }
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        public async Task<string> GetWritableUrl(AmphoraModel entity, string fileName)
        {
            var container = GetContainerReference(entity);
            if(await container.CreateIfNotExistsAsync())
            {
                logger.LogInformation($"Created blob container {container.Name}");
            }
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create;

            var blob = container.GetBlockBlobReference(fileName);
            var uri = $"{blob.Uri}{blob.GetSharedAccessSignature(sasConstraints)}";
            return uri;
        }
        public async Task<byte[]> GetDataAsync(AmphoraModel entity, string name)
        {
            return await this.ReadBytesAsync(entity, name);
        }

        public async Task<IEnumerable<string>> ListNamesAsync(AmphoraModel entity)
        {
            return await this.ListBlobsAsync(entity);
        }

        public async Task<byte[]> SetDataAsync(AmphoraModel entity, byte[] data, string name)
        {
            await this.WriteBytesAsync(entity, name, data);
            return data;
        }

        public async Task<string> GetPublicUrl(AmphoraModel entity, string path)
        {
            var container = GetContainerReference(entity);
            return await GetReadonlyUrlWithSasToken(container, path);
        }

        private CloudBlobContainer GetContainerReference(AmphoraModel amphora)
        {
            return cloudBlobClient.GetContainerReference(GetContainerName(amphora));
        }

        private string GetContainerName(AmphoraModel amphora)
        {
            return $"amphora-{amphora.Id}";
        }

        public async Task<IList<string>> ListBlobsAsync(AmphoraModel entity)
        {
            var container = GetContainerReference(entity);
            if (!await container.ExistsAsync())
            {
                return new List<string>(); // empty
            }
            return await base.ListNamesAsync(container);
        }
    }
}