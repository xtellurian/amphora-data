using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amphora.Common.Configuration.Options;
using Amphora.Migrate.Cosmos;
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
        private readonly CosmosCollectionMigrator migrator;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<CosmosMigrationOptions> options, CosmosCollectionMigrator migrator)
        {
            this.logger = logger;
            this.options = options;

            this.migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                await migrator.MoveContainerAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error migrating cosmos, {ex.Message}", ex);
            }
            logger.LogInformation("Stopping");
            await this.StopAsync(stoppingToken);
            Environment.Exit(0);
        }
    }
}
