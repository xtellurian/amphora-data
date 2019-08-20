using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Amphora.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables();
                    var builtConfig = config.Build();
                    KeyVaultConfigProvider(config, builtConfig);
                })
                .UseStartup<Startup>();

        public const string kvUri = "kvUri";
        private static void KeyVaultConfigProvider(IConfigurationBuilder config, IConfigurationRoot builtConfig)
        {
            if (!string.IsNullOrEmpty(builtConfig[kvUri]))
            {
                System.Console.WriteLine($"Using KeyVault {builtConfig[kvUri]} as Config Provider");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault($"{builtConfig[kvUri]}",
                    keyVaultClient,
                    new DefaultKeyVaultSecretManager());
                    System.Console.WriteLine("KeyVault Configuration Loaded!");
            }
            else 
            {
                System.Console.WriteLine("No KeyVault as config provider");
            }
        }
    }
}
