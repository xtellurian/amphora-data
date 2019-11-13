using Amphora.Common.Configuration.Options;

namespace Amphora.Migrate.Options
{
    public class StorageMigrationOptions
    {
        public StorageOptionsWrapper? Source { get; set; }
        public StorageOptionsWrapper? Sink { get; set; }
    }

    public class StorageOptionsWrapper
    {
        public AzureStorageAccountOptions? Storage { get; set; }
    }
}