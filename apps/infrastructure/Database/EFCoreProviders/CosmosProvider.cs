using Amphora.Common.Configuration.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amphora.Infrastructure.Database.EFCoreProviders
{
    public static class CosmosProvider
    {
        public static void UseCosmos<TContext>(this IServiceCollection services, CosmosOptions options, ILogger? logger = null) where TContext : DbContext
        {
            services.AddDbContext<TContext>(optionsBuilder =>
               {
                   optionsBuilder.UseCosmos(options.Endpoint, options.PrimaryKey, options.Database);
                   optionsBuilder.UseLazyLoadingProxies();
               });

            var msg = $"Using Cosmos DB, Endpoint: {options.Endpoint}, Database: {options.Database}";
            if (logger != null)
            {
                logger.LogInformation(msg);
            }
            else
            {
                System.Console.WriteLine(msg);
            }
        }
    }
}
