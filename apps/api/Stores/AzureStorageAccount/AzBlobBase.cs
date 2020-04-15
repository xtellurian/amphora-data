using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Configuration.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public abstract class AzBlobBase<T> where T : class
    {
        protected readonly CloudBlobClient cloudBlobClient;
        protected readonly ILogger<AzBlobBase<T>> logger;

        protected AzBlobBase(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AzBlobBase<T>> logger)
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

        protected abstract CloudBlobContainer GetContainerReference(T entity);
        protected async Task<bool> BlobExistsAsync(CloudBlobContainer container, string path)
        {
            return await container.GetBlobReference(path).ExistsAsync();
        }

        public async Task<long> GetContainerSizeAsync(T entity)
        {
            var container = GetContainerReference(entity);
            if (!await container.ExistsAsync())
            {
                return 0; // empty
            }

            return container.ListBlobs(useFlatBlobListing: true)
                .OfType<CloudBlockBlob>()
                .Sum(b => b.Properties.Length);
        }

        protected async Task<IList<string>> ListNamesAsync(CloudBlobContainer container)
        {
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            var names = new List<string>();
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
                    await blob.FetchAttributesAsync();
                    names.Add(name);
                }
            }
            while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return names;
        }

        protected async Task<string> GetReadonlyUrlWithSasToken(CloudBlobContainer container, string path)
        {
            if (await container.ExistsAsync())
            {
                var blob = container.GetBlockBlobReference(path);
                var sasToken = GetBlobSasToken(container, blob, SharedAccessBlobPermissions.Read);
                return blob.Uri + sasToken;
            }
            else
            {
                return null;
            }
        }

        private static string GetBlobSasToken(CloudBlobContainer container, CloudBlockBlob blob, SharedAccessBlobPermissions permissions, string policyName = null)
        {
            string sasBlobToken;

            if (policyName == null)
            {
                var adHocSas = CreateAdHocSasPolicy(permissions);

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSas);
            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            return sasBlobToken;
        }

        private static SharedAccessBlobPolicy CreateAdHocSasPolicy(SharedAccessBlobPermissions permissions)
        {
            // Create a new access policy and define its constraints.
            // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
            // to construct a shared access policy that is saved to the container's shared access policies.

            return new SharedAccessBlobPolicy()
            {
                // Set start time to five minutes before now to avoid clock skew.
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Permissions = permissions
            };
        }
    }
}