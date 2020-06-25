using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Amphora.Infrastructure.Stores.AzureStorage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class PlatformCacheBlobStore : AzBlobBase<PlatformCacheBlobStore>, IBlobCache
    {
        private const string ContainerName = "platformcache";
        public PlatformCacheBlobStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<PlatformCacheBlobStore> logger)
        : base(options, logger)
        { }

        public async Task<T> TryGetValue<T>(string key) where T : class
        {
            var container = GetContainerReference(this);
            if (!await container.ExistsAsync())
            {
                return default(T); // empty
            }

            try
            {
                var blob = container.GetBlobClient(key);
                var buffer = new MemoryStream();
                var d = await blob.DownloadToAsync(buffer);
                var data = buffer.ToArray();
                var s = Encoding.UTF8.GetString(data);
                var value = JsonConvert.DeserializeObject<T>(s);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value) where T : class
        {
            // 1 container per amphora
            var container = GetContainerReference(this);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(key);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{key} already exists. ${blob.Uri}. Deleting");
                await blob.DeleteAsync();
            }

            var serialised = JsonConvert.SerializeObject(value);
            var bytes = Encoding.ASCII.GetBytes(serialised);
            var stream = new MemoryStream(bytes);
            var response = await blob.UploadAsync(stream);
        }

        // self referential == singleton
        protected override BlobContainerClient GetContainerReference(PlatformCacheBlobStore entity)
        {
            // base class workaround
            return blobServiceClient.GetBlobContainerClient(ContainerName);
        }
    }
}