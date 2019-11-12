using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Amphora.Common.Configuration
{
    public class KeyVaultConfigProvider
    {
        private const string kvUri = "kvUri";
        private const string disableKv = "disableKv";
        public static void Configure(IConfigurationBuilder config, IConfigurationRoot builtConfig, string vaultUri = null, IKeyVaultSecretManager manager = null)
        {
            vaultUri ??= builtConfig[kvUri];
            manager ??= new DefaultKeyVaultSecretManager();

            if (!string.IsNullOrEmpty(vaultUri) && string.IsNullOrEmpty(builtConfig[disableKv]))
            {
                System.Console.WriteLine($"Using KeyVault {builtConfig[kvUri]} as Config Provider");
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
        }
    }
}