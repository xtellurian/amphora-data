using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Infrastructure.Stores.AzureStorage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class OrganisationBlobStore : AzBlobBase<OrganisationModel>, IBlobStore<OrganisationModel>
    {
        public OrganisationBlobStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<OrganisationBlobStore> logger)
         : base(options, logger)
        {
        }

        public async Task<byte[]> ReadBytesAsync(OrganisationModel org, string path)
        {
            var container = GetContainerReference(org);
            if (await container.ExistsAsync() && await BlobExistsAsync(container, path))
            {
                var blob = container.GetBlobClient(path);
                var buffer = new MemoryStream();
                await blob.DownloadToAsync(buffer);
                return buffer.ToArray();
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ExistsAsync(OrganisationModel org, string path)
        {
            var container = GetContainerReference(org);
            if (await container.ExistsAsync())
            {
                return await BlobExistsAsync(container, path);
            }
            else
            {
                return false;
            }
        }

        public async Task WriteAsync(OrganisationModel org, string path, Stream content)
        {
            var container = GetContainerReference(org);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
            }

            await blob.UploadAsync(content);
        }

        public async Task WriteBytesAsync(OrganisationModel org, string path, byte[] bytes)
        {
            var container = GetContainerReference(org);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
            }

            var stream = new MemoryStream(bytes);
            await blob.UploadAsync(stream);
        }

        public async Task<DateTimeOffset?> LastModifiedAsync(OrganisationModel entity)
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

        public async Task<string> GetPublicUrl(OrganisationModel entity, string path)
        {
            var container = GetContainerReference(entity);
            return await GetReadonlyUrlWithSasToken(container, path);
        }

        protected override BlobContainerClient GetContainerReference(OrganisationModel org)
        {
            return blobServiceClient.GetBlobContainerClient(GetContainerName(org));
        }

        private string GetContainerName(OrganisationModel org)
        {
            return $"organisation-{org.Id}";
        }

        public Task<string> GetWritableUrl(OrganisationModel entity, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(OrganisationModel entity, string path)
        {
            var container = GetContainerReference(entity);
            var blob = container.GetBlobClient(path);
            return await blob.DeleteIfExistsAsync();
        }
    }
}