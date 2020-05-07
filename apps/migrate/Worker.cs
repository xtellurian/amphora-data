using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Migrators;
using Amphora.Migrate.Migrators.Cosmos;
using Amphora.Migrate.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IOptionsMonitor<CosmosMigrationOptions> options;
        private Version_0_10_0 V0_10 { get; }

        public Worker(ILogger<Worker> logger,
                      IOptionsMonitor<CosmosMigrationOptions> options,
                      Version_0_10_0 v0_10)
        {
            this.logger = logger;
            this.options = options;
            this.V0_10 = v0_10;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // run each of these separately
            // await RunMigratorsAsync("Import Cosmos from Prod", cosmosMigrator); // run this to get things into dev env. no need to run targetting prod
            // await RunMigratorsAsync("Move and refactor to IdentityContext", identityModelMigrator); // run this to move ApplicationUsers into IdentityContext
            // await RunMigratorsAsync("Add data models to AmphoraContext", userDataModelMigrator); // run this to add UserDataModels to AmphoraContext
            // await RunMigratorsAsync("Delete Old ApplicationUser documents", cosmosDeleter); // run this to add UserDataModels to AmphoraContext
            await RunMigratorsAsync("Update to V 0.10", V0_10);
            logger.LogInformation("Stopping");
            await this.StopAsync(stoppingToken);
            Environment.Exit(0);
        }

        private async Task RunMigratorsAsync(string name, params IMigrator[] migrators)
        {
            logger.LogInformation($"Started {name} at {DateTimeOffset.Now}");
            var migrations = new List<Task>();
            foreach (var m in migrators)
            {
                migrations.Add(m.MigrateAsync());
            }

            try
            {
                await Task.WhenAll(migrations);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Migrating, {ex.Message}", ex);
            }

            logger.LogInformation($"Finished! {name} at {DateTimeOffset.Now}");
        }
    }
}
