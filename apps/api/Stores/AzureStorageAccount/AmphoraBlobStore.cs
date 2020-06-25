using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Infrastructure.Stores.AzureStorage;
using Azure.Storage.Blobs;
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

            var blob = container.GetBlobClient(path);
            var buffer = new MemoryStream();
            await blob.DownloadToAsync(buffer);
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
            var blob = container.GetBlobClient(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {entity.Id}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {entity.Id}. ${blob.Uri}");
            }

            var stream = new MemoryStream(bytes);
            await blob.UploadAsync(stream);
        }

        public async Task<DateTimeOffset?> LastModifiedAsync(AmphoraModel entity)
        {
            // 1 container per amphora
            var container = GetContainerReference(entity);
            if (await container.ExistsAsync())
            {
                var properties = await container.GetPropertiesAsync();
                return properties.Value.LastModified;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetWritableUrl(AmphoraModel entity, string fileName)
        {
            var container = GetContainerReference(entity);
            var created = await container.CreateIfNotExistsAsync();

            logger.LogInformation($"Blob container {container.Name} last modified at {created.Value.LastModified}");

            var uri = await GetWritableUrlWithSasToken(container, fileName);

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

            var blobs = await ListBlobsAsync(container);
            files.AddRange(blobs.Select(_ => new AzureBlobAmphoraFileReference(_, Path.GetFileName(_.Name))));

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

        protected override BlobContainerClient GetContainerReference(AmphoraModel amphora)
        {
            return blobServiceClient.GetBlobContainerClient(GetContainerName(amphora));
        }

        private string GetContainerName(AmphoraModel amphora)
        {
            return $"amphora-{amphora.Id}";
        }

        public async Task<bool> DeleteAsync(AmphoraModel entity, string path)
        {
            var container = GetContainerReference(entity);
            var blob = container.GetBlobClient(path);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task WriteAsync(AmphoraModel entity, string path, Stream content)
        {
            var container = GetContainerReference(entity);
            var blob = container.GetBlobClient(path);
            await blob.UploadAsync(content);
        }
    }
}