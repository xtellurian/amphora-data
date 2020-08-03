using Amphora.Common.Configuration.Options;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Database.Cache
{
    public static class CosmosCache
    {
        public static void UseCosmosCache(this IServiceCollection services, CosmosOptions cosmosOptions)
        {
            services.AddCosmosCache((cacheOptions) =>
            {
                cacheOptions.ContainerName = "cache";
                cacheOptions.DatabaseName = cosmosOptions.Database;
                cacheOptions.ClientBuilder = new CosmosClientBuilder(cosmosOptions.GenerateConnectionString(cosmosOptions.PrimaryKey));

                cacheOptions.CreateIfNotExists = true;
            });
        }
    }
}