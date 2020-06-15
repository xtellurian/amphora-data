using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Identity.Stores.EFCore;
using Amphora.Infrastructure.Database.Contexts;
using Amphora.Infrastructure.Database.EFCoreProviders;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Stores.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static void RegisterApplications(this IServiceCollection services, bool isDevelopment = false)
        {
            services.UseInMemory<ApplicationsContext>();
            RegisterApplicationsServices(services, isDevelopment);
        }

        public static void RegisterApplications(this IServiceCollection services, CosmosOptions cosmosOptions, bool isDevelopment = false)
        {
            services.UseCosmos<ApplicationsContext>(cosmosOptions);
            RegisterApplicationsServices(services, isDevelopment);
        }

        public static void RegisterApplications(this IServiceCollection services, SqlServerOptions sqlOptions, bool isDevelopment = false)
        {
            services.UseSqlServer<ApplicationsContext>(sqlOptions);
            RegisterApplicationsServices(services, isDevelopment);
        }

        private static void RegisterApplicationsServices(IServiceCollection services, bool isDevelopment)
        {
            services.AddScoped<IEntityStore<ApplicationModel>, ApplicationModelEFStore>();
            services.AddScoped<IEntityStore<ApplicationLocationModel>, ApplicationLocationModelEFStore>();
            if (isDevelopment)
            {
                services.Decorate<IEntityStore<ApplicationModel>, ApplicationStoreOurAppsDecoratorDevelop>();
                services.Decorate<IEntityStore<ApplicationLocationModel>, ApplicationLocationsStoreOurAppsDecoratorDevelop>();
            }
            else
            {
                services.Decorate<IEntityStore<ApplicationModel>, ApplicationStoreOurAppsDecoratorProduction>();
                services.Decorate<IEntityStore<ApplicationLocationModel>, ApplicationLocationsStoreOurAppsDecoratorProduction>();
            }
        }
    }
}