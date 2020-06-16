using Microsoft.Extensions.Configuration;

namespace Amphora.Common.Configuration
{
    public class AzureAppConfigurationConfigProvider
    {
        private const string ConnectionStringKey = "AzureAppConfig:ReadOnly:PrimaryConnectionString";
        private const string DisableKv = "disableKv";
        public static IConfigurationBuilder Configure(IConfigurationBuilder config,
                                     IConfigurationRoot settings)
        {
            var connectionString = settings[ConnectionStringKey];

            if (!string.IsNullOrEmpty(connectionString) && string.IsNullOrEmpty(settings[DisableKv]))
            {
                System.Console.WriteLine($"Using Azure App Config {settings[ConnectionStringKey]?.Substring(5, 15)}... as Config Provider");
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString);
                    options.UseFeatureFlags();
                });
                System.Console.WriteLine("Connected to Azure App Configuration!");
            }
            else
            {
                System.Console.WriteLine("Not using Azure App Configuration");
            }

            return config;
        }
    }
}