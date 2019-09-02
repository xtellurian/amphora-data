using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores
{
    public class AzBlobAmphoraDataStore : IDataStore<Amphora.Common.Models.Amphora, byte[]>
    {
        private readonly ILogger<AzBlobAmphoraDataStore> logger;
        private readonly CloudBlobClient cloudBlobClient;

        public AzBlobAmphoraDataStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AzBlobAmphoraDataStore> logger)
        {
            this.logger = logger;

            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(options.CurrentValue.StorageConnectionString, out storageAccount))
            {
                this.cloudBlobClient = storageAccount.CreateCloudBlobClient();
            }
            else
            {
                logger.LogCritical("Couldn't Parse Storage Account connection string");
                throw new ArgumentNullException(nameof(options.CurrentValue.StorageConnectionString));
            }
        }
        public async Task<byte[]> GetDataAsync(Common.Models.Amphora entity, string name)
        {
            var container = cloudBlobClient.GetContainerReference(entity.AmphoraId);
            if (!await container.ExistsAsync())
            {
                return null; // empty
            }

            var blob = container.GetBlockBlobReference(name);
            var buffer = new MemoryStream();
            await blob.DownloadToStreamAsync(buffer);
            return buffer.ToArray();
        }

        public async Task<IEnumerable<string>> ListNamesAsync(Common.Models.Amphora entity)
        {
            var container = cloudBlobClient.GetContainerReference(entity.AmphoraId);
            if (!await container.ExistsAsync())
            {
                return new List<string>(); // empty
            }
            var names = new List<string>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await container.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    names.Add(Path.GetFileName(item.Uri.LocalPath));
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return names;
        }

        public async Task<byte[]> SetDataAsync(Common.Models.Amphora entity, byte[] data, string name)
        {
            // 1 container per amphora
            var container = cloudBlobClient.GetContainerReference(entity.AmphoraId);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(name);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{name} already exists in {entity.AmphoraId}. ${blob.Uri}");
                throw new ArgumentException($"{name} already exists in {entity.AmphoraId}. ${blob.Uri}");
            }
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return data;
        }
    }
}