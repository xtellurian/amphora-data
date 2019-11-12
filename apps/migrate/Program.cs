using Amphora.Common.Configuration;
using Amphora.Migrate.Migrators;
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
                    KeyVaultConfigProvider.Configure(config, builtConfig, sourceVault, new SectionReplacementSecretManager(sourceVault, "Source", "Cosmos"));
                    KeyVaultConfigProvider.Configure(config, builtConfig, sinkVault, new SectionReplacementSecretManager(sinkVault, "Sink", "Cosmos"));

                    Configuration = config.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<BlobMigrator>();
                    services.Configure<StorageMigrationOptions>(Configuration);

                    services.AddSingleton<CosmosCollectionMigrator>();
                    services.Configure<CosmosMigrationOptions>(Configuration);
                });
    }
}
