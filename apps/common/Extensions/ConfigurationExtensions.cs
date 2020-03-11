using Microsoft.Extensions.Configuration;

namespace Amphora.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsPersistentStores(this IConfiguration configuration)
        {
            return configuration["PersistentStores"] == "true";
        }
    }
}