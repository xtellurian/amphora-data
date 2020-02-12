using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Configuration.Options;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Stores.AzureStorageAccount
{
    public class PlatformCacheBlobStore : AzBlobBase, IBlobCache
    {
        private const string ContainerName = "platformcache";
        public PlatformCacheBlobStore(IOptionsMonitor<AzureStorageAccountOptions> options, ILogger<AmphoraBlobStore> logger) : base(options, logger)
        {
        }

        private CloudBlobContainer GetContainerReference()
        {
            return cloudBlobClient.GetContainerReference(ContainerName);
        }

        public async Task<T> TryGetValue<T>(string key) where T : class
        {
            var container = GetContainerReference();
            if (!await container.ExistsAsync())
            {
                return default(T); // empty
            }

            try
            {
                var blob = container.GetBlockBlobReference(key);
                var buffer = new MemoryStream();
                await blob.DownloadToStreamAsync(buffer);
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
            var container = GetContainerReference();
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(key);
            if (await blob.ExistsAsync())
            {
                logger.LogError($"{key} already exists. ${blob.Uri}. Deleting");
                await blob.DeleteAsync();
            }

            var serialised = JsonConvert.SerializeObject(value);
            var bytes = Encoding.ASCII.GetBytes(serialised);
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }
    }
}