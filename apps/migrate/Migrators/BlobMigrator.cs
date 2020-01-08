using System;
using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    public class BlobMigrator : IMigrator
    {
        private readonly ILogger<BlobMigrator> logger;
        private readonly StorageMigrationOptions options;

        public BlobMigrator(ILogger<BlobMigrator> logger, IOptionsMonitor<StorageMigrationOptions> options)
        {
            this.logger = logger;
            this.options = options.CurrentValue;
            if (options.CurrentValue?.Source?.Storage?.StorageConnectionString == null)
            {
                throw new NullReferenceException("Source Connection String cannot be null");
            }

            if (options.CurrentValue?.Sink?.Storage?.StorageConnectionString == null)
            {
                throw new NullReferenceException("Sink Connection String cannot be null");
            }
        }

        public async Task MigrateAsync()
        {
            // create the blob client for the destination storage account
            var sourceAccount = CloudStorageAccount.Parse(options.Source?.Storage?.StorageConnectionString);
            var sourceClient = sourceAccount.CreateCloudBlobClient();
            var sinkAccount = CloudStorageAccount.Parse(options.Sink?.Storage?.StorageConnectionString);
            var sinkClient = sinkAccount.CreateCloudBlobClient();

            foreach (var sourceContainer in sourceClient.ListContainers())
            {
                // create a 2 hour SAS token for the source file
                var sas = sourceContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTimeOffset.Now.AddMinutes(-5),
                    SharedAccessExpiryTime = DateTimeOffset.Now.AddHours(2)
                });

                var sinkContainer = sinkClient.GetContainerReference(sourceContainer.Name);
                await sinkContainer.CreateIfNotExistsAsync();

                foreach (var sourceBlob in sourceContainer.ListBlobs(useFlatBlobListing: true))
                {
                    var sourceUri = new Uri(sourceBlob.Uri + sas);
                    var sourcePath = sourceBlob.Uri.LocalPath.TrimStart('/').Substring(sourceContainer.Name.Length).TrimStart('/'); // fixes path
                    var sinkBlob = sinkContainer.GetBlockBlobReference(sourcePath);
                    logger.LogInformation($"{sourceBlob.Uri} -> {sinkBlob.Uri}");
                    await sinkBlob.StartCopyAsync(sourceUri);
                }
            }
        }
    }
}