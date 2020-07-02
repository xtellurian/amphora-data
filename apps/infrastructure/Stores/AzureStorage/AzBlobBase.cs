using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Configuration.Options;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Infrastructure.Stores.AzureStorage
{
    public abstract class AzBlobBase<T> where T : class
    {
        protected readonly ILogger<AzBlobBase<T>> logger;
        protected readonly BlobServiceClient blobServiceClient;
        private string? accountKey;

        protected AzBlobBase(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AzBlobBase<T>> logger)
        {
            this.logger = logger;
            // Check whether the connection string can be parsed.
            this.blobServiceClient = new BlobServiceClient(options.CurrentValue.StorageConnectionString);
            this.accountKey = options.CurrentValue.Key;
        }

        protected abstract BlobContainerClient GetContainerReference(T entity);
        protected async Task<bool> BlobExistsAsync(BlobContainerClient container, string path)
        {
            return await container.GetBlobClient(path).ExistsAsync();
        }

        public async Task<long> GetContainerSizeAsync(T entity)
        {
            var container = GetContainerReference(entity);
            if (!await container.ExistsAsync())
            {
                return 0; // empty
            }

            long sum = 0;
            await foreach (var blob in container.GetBlobsAsync())
            {
                sum += blob.Properties.ContentLength ?? 0;
            }

            return sum;
        }

        protected async Task<int> CountBlobsAsync(BlobContainerClient container,
                                                             string? prefix = null)
        {
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            string? continuationToken = null;
            var count = 0;
            try
            {
                // Call the listing operation and enumerate the result segment.
                // When the continuation token is empty, the last segment has been returned
                // and execution can exit the loop.
                do
                {
                    var resultSegment = container.GetBlobsAsync(prefix: prefix)
                        .AsPages(continuationToken);

                    await foreach (var blobPage in resultSegment)
                    {
                        count += blobPage.Values.Count;

                        // Get the continuation token and loop until it is empty.
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                while (continuationToken != "");
            }
            catch (Azure.RequestFailedException e)
            {
                logger.LogCritical("Error Listing Blobs", e);
                throw e;
            }

            return count;
        }

        protected async Task<IList<BlobItem>> ListBlobsAsync(BlobContainerClient container,
                                                             string? prefix = null,
                                                             bool includeMetadata = true,
                                                             int skip = 0,
                                                             int? take = null)
        {
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            string? continuationToken = null;
            var results = new List<BlobItem>();
            var traits = includeMetadata ? BlobTraits.Metadata : BlobTraits.None;
            int skipped = 0;
            try
            {
                // Call the listing operation and enumerate the result segment.
                // When the continuation token is empty, the last segment has been returned
                // and execution can exit the loop.
                do
                {
                    var resultSegment = container.GetBlobsAsync(prefix: prefix, traits: traits)
                        .AsPages(continuationToken, take);

                    await foreach (var blobPage in resultSegment)
                    {
                        foreach (var blobItem in blobPage.Values)
                        {
                            if (skipped++ >= skip && results.Count < take)
                            {
                                results.Add(blobItem);
                            }
                        }

                        // Get the continuation token and loop until it is empty.
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                while (continuationToken != "");
            }
            catch (Azure.RequestFailedException e)
            {
                logger.LogCritical("Error Listing Blobs", e);
                throw e;
            }

            return results;
        }

        protected async Task<string?> GetReadonlyUrlWithSasToken(BlobContainerClient container, string path)
        {
            if (await container.ExistsAsync())
            {
                var blob = container.GetBlobClient(path);
                var uri = GetBlobSasUriAsync(blob, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(15));
                return uri.ToString();
            }
            else
            {
                return null;
            }
        }

        protected async Task<string?> GetWritableUrlWithSasToken(BlobContainerClient container, string path)
        {
            if (await container.ExistsAsync())
            {
                var blob = container.GetBlobClient(path);
                var uri = GetBlobSasUriAsync(blob, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(15), BlobSasPermissions.Write);
                return uri.ToString();
            }
            else
            {
                return null;
            }
        }

        private Uri GetBlobSasUriAsync(BlobClient blob,
                                                   DateTimeOffset startsOn,
                                                   DateTimeOffset endsOn,
                                                   BlobSasPermissions permission = BlobSasPermissions.Read)
        {
            // var key = await this.blobServiceClient.GetUserDelegationKeyAsync(startsOn, endsOn);
            // var key = await blobServiceClient.GetUserDelegationKeyAsync
            // Create a SAS token that's valid for one hour.
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b",
                StartsOn = startsOn,
                ExpiresOn = endsOn
            };

            var credentials = new StorageSharedKeyCredential(blob.AccountName, this.accountKey);
            // Specify read permissions for the SAS.
            sasBuilder.SetPermissions(permission);

            // Use the key to get the SAS token.
            var sasToken = sasBuilder.ToSasQueryParameters(credentials).ToString();

            // Construct the full URI, including the SAS token.
            UriBuilder fullUri = new UriBuilder()
            {
                Scheme = "https",
                Host = string.Format("{0}.blob.core.windows.net", blob.AccountName),
                Path = string.Format("{0}/{1}", blob.BlobContainerName, blob.Name),
                Query = sasToken
            };

            return fullUri.Uri;
        }
    }
}