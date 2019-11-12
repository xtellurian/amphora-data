using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Models.Organisations;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class OrganisationBlobStore : AzBlobBase, IBlobStore<OrganisationModel>
    {
        public OrganisationBlobStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AzBlobBase> logger) : base(options, logger)
        {

        }

        public async Task<byte[]> ReadBytesAsync(OrganisationModel org, string path)
        {
            var container = GetContainerReference(org);
            if (await container.ExistsAsync())
            {
                var blob = container.GetBlockBlobReference(path);
                var buffer = new MemoryStream();
                await blob.DownloadToStreamAsync(buffer);
                return buffer.ToArray();
            }
            else
            {
                return null;
            }
        }

        public async Task WriteBytesAsync(OrganisationModel org, string path, byte[] bytes)
        {
            var container = GetContainerReference(org);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(path);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
                throw new ArgumentException($"{path} already exists in {GetContainerName(org)}. ${blob.Uri}");
            }
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        public async Task<IList<string>> ListBlobsAsync(OrganisationModel entity)
        {
            var container = GetContainerReference(entity);
            if (await container.ExistsAsync())
            {
                return await base.ListNamesAsync(container);
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

        private CloudBlobContainer GetContainerReference(OrganisationModel org)
        {
            return cloudBlobClient.GetContainerReference(GetContainerName(org));
        }

        private string GetContainerName(OrganisationModel org)
        {
            return $"organisation-{org.Id}";
        }

        public Task<string> GetWritableUrl(OrganisationModel entity, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}