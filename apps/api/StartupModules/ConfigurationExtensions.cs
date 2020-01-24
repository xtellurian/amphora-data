using Microsoft.Extensions.Configuration;

namespace Amphora.Api.StartupModules
{
    public static class ConfigurationExtensions
    {
        public static bool IsPersistentStores(this IConfiguration configuration)
        {
            return configuration["PersistentStores"] == "true";
        }
    }
}