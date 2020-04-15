using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Infrastructure.Stores.AzureStorage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class AmphoraBlobStore : AzBlobBase<AmphoraModel>, IAmphoraBlobStore
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

        public async Task<bool> ExistsAsync(AmphoraModel amphora, string path)
        {
            var container = GetContainerReference(amphora);
            if (await container.ExistsAsync())
            {
                return await BlobExistsAsync(container, path);
            }
            else
            {
                return false;
            }
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

        public async Task<DateTimeOffset?> LastModifiedAsync(AmphoraModel entity)
        {
            // 1 container per amphora
            var container = GetContainerReference(entity);
            if (await container.ExistsAsync())
            {
                await container.FetchAttributesAsync();
                return container.Properties.LastModified;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetWritableUrl(AmphoraModel entity, string fileName)
        {
            var container = GetContainerReference(entity);
            if (await container.CreateIfNotExistsAsync())
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

        public async Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity)
        {
            var container = GetContainerReference(entity);
            var files = new List<IAmphoraFileReference>();
            if (!await container.ExistsAsync())
            {
                return files;
            }

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await container.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (var item in results.Results)
                {
                    var name = Path.GetFileName(item.Uri.LocalPath);
                    var blob = container.GetBlobReference(name);
                    files.Add(new AzureBlobAmphoraFileReference(blob, name));
                }
            }
            while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return files;
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

        protected override CloudBlobContainer GetContainerReference(AmphoraModel amphora)
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

            return await ListNamesAsync(container);
        }

        public async Task<Stream> GetWritableStreamAsync(AmphoraModel amphora, string path)
        {
            var container = GetContainerReference(amphora);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {GetContainerName(amphora)}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {GetContainerName(amphora)}. ${blob.Uri}");
            }

            return await blob.OpenWriteAsync();
        }

        public async Task<bool> DeleteAsync(AmphoraModel entity, string path)
        {
            var container = GetContainerReference(entity);
            var blob = container.GetBlockBlobReference(path);
            return await blob.DeleteIfExistsAsync();
        }
    }
}