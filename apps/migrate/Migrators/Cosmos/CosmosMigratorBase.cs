using System.Threading.Tasks;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    public abstract class CosmosMigratorBase
    {
        private readonly CosmosMigrationOptions options;
        protected readonly ILogger<CosmosMigratorBase> logger;
        protected ContainerResponse? sinkContainerProperties;
        protected ContainerResponse? sourceContainerProperties;
        protected Database? sinkDatabase;

        public CosmosMigratorBase(IOptionsMonitor<CosmosMigrationOptions> options,
                                            ILogger<CosmosMigratorBase> logger)
        {
            this.options = options.CurrentValue;
            this.logger = logger;
        }

        public async Task<Container> GetSourceContainerAsync()
        {
            var clientOptions = new CosmosClientOptions() { AllowBulkExecution = false };
            var sourceClient = new CosmosClient(options.Source?.Cosmos?.GenerateConnectionString(options.GetSource()?.PrimaryKey), clientOptions);
            var sourceContainer = sourceClient.GetContainer(options.GetSource()?.Database, options.Source?.Cosmos?.Container);
            this.sourceContainerProperties = await sourceContainer.ReadContainerAsync();

            return sourceContainer;
        }

        public async Task<Container> GetSinkContainerAsync()
        {
            var clientOptions = new CosmosClientOptions() { AllowBulkExecution = false };
            var sinkClient = new CosmosClient(options.Sink?.Cosmos?.GenerateConnectionString(options.GetSink()?.PrimaryKey), clientOptions);
            this.sinkDatabase = sinkClient.GetDatabase(options.GetSink()?.Database);
            var sinkContainer = sinkDatabase.GetContainer(options.GetSink()?.Container);
            this.sinkContainerProperties = await sinkContainer.ReadContainerAsync();

            return sinkContainer;
        }
    }
}