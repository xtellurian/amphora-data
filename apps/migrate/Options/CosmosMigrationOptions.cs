using Amphora.Common.Configuration.Options;

namespace Amphora.Migrate.Options
{
    public class CosmosMigrationOptions
    {
        public CosmosOptions? GetSource() => this.Source?.Cosmos;
        public CosmosOptions? GetSink() => this.Sink?.Cosmos;
        public CosmosOptionsWrapper? Source { get; set; }
        public CosmosOptionsWrapper? Sink { get; set; }

        public bool Upsert { get; set; }
    }

    public class CosmosOptionsWrapper
    {
        public CosmosOptions? Cosmos { get; set; }
    }
}