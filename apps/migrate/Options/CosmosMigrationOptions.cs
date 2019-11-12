using Amphora.Common.Configuration.Options;

namespace Amphora.Migrate.Options
{
    public class CosmosMigrationOptions
    {
        public CosmosOptions? Source { get; set; }
        public CosmosOptions? Sink { get; set; }

    }
}