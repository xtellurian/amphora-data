using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amphora.Migrate.Migrators;
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
        private readonly CosmosCollectionMigrator cosmosMigrator;
        private readonly BlobMigrator blobMigrator;
        private readonly TsiMigrator tsiMigrator;

        public Worker(ILogger<Worker> logger,
                      IOptionsMonitor<CosmosMigrationOptions> options,
                      CosmosCollectionMigrator cosmosMigrator,
                      BlobMigrator blobMigrator,
                      TsiMigrator tsiMigrator
                      )
        {
            this.logger = logger;
            this.options = options;

            this.cosmosMigrator = cosmosMigrator ?? throw new ArgumentNullException(nameof(cosmosMigrator));
            this.blobMigrator = blobMigrator ?? throw new ArgumentNullException(nameof(blobMigrator));
            this.tsiMigrator = tsiMigrator ?? throw new ArgumentNullException(nameof(tsiMigrator));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var migrations = new List<Task>();
            migrations.Add(cosmosMigrator.MigrateAsync());
            migrations.Add(blobMigrator.MigrateAsync());
            migrations.Add(tsiMigrator.MigrateAsync());
            try
            {
                await Task.WhenAll(migrations);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Migrating, {ex.Message}", ex);
            }
            logger.LogInformation("Stopping");
            await this.StopAsync(stoppingToken);
            Environment.Exit(0);
        }
    }
}
