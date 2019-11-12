using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Configuration;
using Amphora.Common.Configuration.Options;
using Amphora.Migrate.Cosmos;
using Amphora.Migrate.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Migrate
{
    public class Program
    {
        private static IConfiguration? Configuration;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables();
                    var builtConfig = config.Build();
                    var sourceVault = builtConfig["sourceKvUri"];
                    var sinkVault = builtConfig["sinkKvUri"];
                    KeyVaultConfigProvider.Configure(config, builtConfig, sourceVault, new SectionReplacementSecretManager(sourceVault, "Cosmos", "Source"));
                    KeyVaultConfigProvider.Configure(config, builtConfig, sinkVault, new SectionReplacementSecretManager(sinkVault, "Cosmos", "Sink"));
                    Configuration = config.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<CosmosCollectionMigrator>();
                    services.Configure<CosmosMigrationOptions>(Configuration);
                });
    }
}
