using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Services
{
    public static class DataProtection
    {
        private const string KeyName = "dataprotection";
        public static void RegisterKeyVaultWithBlobDataProtection(this IServiceCollection services,
                                                                  string azureStorageConnectionString,
                                                                  string containerName,
                                                                  string blobName,
                                                                  string kvUri,
                                                                  string applicationName)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

            if (!kvUri.EndsWith('/'))
            {
                kvUri += "/"; // ensure ends with a slash
            }

            if (CloudStorageAccount.TryParse(azureStorageConnectionString, out var storageAccount) && !string.IsNullOrEmpty(kvUri))
            {
                System.Console.WriteLine("Enabling KV Backed Blob DataProtection");
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference(containerName);
                container.CreateIfNotExists();
                var keyIdentifier = $"{kvUri}keys/{KeyName}/";
                services.AddDataProtection()
                    .SetApplicationName(applicationName)
                    .PersistKeysToAzureBlobStorage(container, blobName)
                    .ProtectKeysWithAzureKeyVault(keyVaultClient, keyIdentifier);
            }
            else
            {
                System.Console.WriteLine("Not Configuring DataProtection");
            }
        }
    }
}