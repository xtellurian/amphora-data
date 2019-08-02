using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace api
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
                    var builtConfig = config.Build();
                    KeyVaultConfigProvider(config, builtConfig);
                })
                .UseStartup<Startup>();

        public const string kvUri = "kvUri";
        private static void KeyVaultConfigProvider(IConfigurationBuilder config, IConfigurationRoot builtConfig)
        {
            if (!string.IsNullOrEmpty(builtConfig[kvUri]))
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault($"{builtConfig[kvUri]}",
                    keyVaultClient,
                    new DefaultKeyVaultSecretManager());

            }
        }
    }
}
