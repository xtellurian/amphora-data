using Amphora.Common.Configuration.Options;

namespace Amphora.Migrate.Options
{
    public class StorageMigrationOptions
    {
        public AzureStorageAccountOptions? Source { get; set; }
        public AzureStorageAccountOptions? Sink { get; set; }
    }
}