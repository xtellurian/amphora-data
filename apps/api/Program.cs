using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //await CreateWebHostBuilder(args).Build().RunAsync();
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables();
                    var builtConfig = config.Build();
                    KeyVaultConfigProvider(config, builtConfig);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                    })
                    .UseStartup<Startup>();
                });

        public const string kvUri = "kvUri";
        public const string disableKv = "disableKv";
        private static void KeyVaultConfigProvider(IConfigurationBuilder config, IConfigurationRoot builtConfig)
        {
            var vaultUri = builtConfig[kvUri];
            if (!string.IsNullOrEmpty(vaultUri) && string.IsNullOrEmpty(builtConfig[disableKv]))
            {
                System.Console.WriteLine($"Using KeyVault {builtConfig[kvUri]} as Config Provider");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault($"{vaultUri}",
                    keyVaultClient,
                    new DefaultKeyVaultSecretManager());
                try
                {
                    var entry = System.Net.Dns.GetHostEntry(vaultUri);
                }
                catch(System.Exception ex)
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
