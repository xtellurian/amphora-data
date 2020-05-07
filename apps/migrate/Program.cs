using Amphora.Api.Contracts;
using Amphora.Api.Services.Azure;
using Amphora.Common.Configuration;
using Amphora.Common.Contracts;
using Amphora.Common.Options;
using Amphora.Common.Services.Azure;
using Amphora.Migrate.Migrators;
using Amphora.Migrate.Migrators.Cosmos;
using Amphora.Migrate.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Migrate
{
    public class Program
    {
        private static IConfiguration? configuration;
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
                    KeyVaultConfigProvider.Configure(config, builtConfig, sourceVault, new SectionReplacementSecretManager(sourceVault, "Source"));
                    KeyVaultConfigProvider.Configure(config, builtConfig, sinkVault, new SectionReplacementSecretManager(sinkVault, "Sink"));

                    configuration = config.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    if (configuration == null)
                    {
                        throw new System.NullReferenceException("Configuration cannot be null");
                    }

                    services.AddHostedService<Worker>();

                    services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper("RunAs=Developer; DeveloperTool=AzureCli"));

                    services.AddSingleton<BlobMigrator>();
                    services.Configure<StorageMigrationOptions>(configuration);

                    services.AddSingleton<CosmosCollectionMigrator>();
                    services.Configure<CosmosMigrationOptions>(configuration);

                    services.AddSingleton<CosmosDocumentDeleteMigrator>();
                    services.AddSingleton<CosmosUpdateIdentityModelMigrator>();
                    services.AddSingleton<CosmosUserDataModelMigrator>();
                    services.AddSingleton<Version_0_10_0>();

                    services.AddTransient<ITsiService, TsiService>();
                    services.AddSingleton<TsiMigrator>();
                    services.AddTransient<EventHubSender>();
                    services.Configure<TsiOptions>(configuration.GetSection("Source").GetSection("Tsi"));
                    services.Configure<EventHubOptions>(configuration.GetSection("Sink").GetSection("TsiEventHub"));
                });
    }
}
