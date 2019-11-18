using System.Threading.Tasks;

namespace Amphora.Migrate.Contracts
{
    internal interface IMigrator
    {
        Task MigrateAsync();
    }
}