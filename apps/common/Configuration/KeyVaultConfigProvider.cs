using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Amphora.Common.Configuration
{
    public class KeyVaultConfigProvider
    {
        private const string KvUri = "kvUri";
        private const string DisableKv = "disableKv";
        public static IConfigurationBuilder Configure(IConfigurationBuilder config,
                                     IConfigurationRoot builtConfig,
                                     string? vaultUri = null,
                                     IKeyVaultSecretManager? manager = null)
        {
            vaultUri ??= builtConfig[KvUri];
            manager ??= new DefaultKeyVaultSecretManager();

            if (!string.IsNullOrEmpty(vaultUri) && string.IsNullOrEmpty(builtConfig[DisableKv]))
            {
                System.Console.WriteLine($"Using KeyVault {builtConfig[KvUri]} as Config Provider");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault($"{vaultUri}", keyVaultClient, manager);
                try
                {
                    var httpsPrefix = "https://";
                    if (vaultUri.StartsWith(httpsPrefix))
                    {
                        var kvHost = vaultUri.Substring(httpsPrefix.Length).Trim('/');
                        var entry = System.Net.Dns.GetHostEntry(kvHost);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"KeyVault DNS Lookup Failed!");
                    throw ex;
                }

                System.Console.WriteLine("KeyVault Configuration Loaded!");
            }
            else
            {
                System.Console.WriteLine("No KeyVault as config provider");
            }

            return config;
        }
    }
}