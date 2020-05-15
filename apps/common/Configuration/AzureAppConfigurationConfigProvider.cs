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
                config.AddAzureAppConfiguration(connectionString);
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString);
                    //    .ConfigureRefresh(refresh =>
                    //         {
                    //             refresh.Register("Api:Settings:Sentinel", refreshAll: true)
                    //                    .SetCacheExpiration(new System.TimeSpan(0, 5, 0));
                    //         });
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