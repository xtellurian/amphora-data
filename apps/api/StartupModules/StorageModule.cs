using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Api.Stores;
using Amphora.Api.Stores.Cosmos;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class StorageModule
    {
        public StorageModule(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IHostingEnvironment HostingEnvironment { get; }

        private readonly IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureStorageAccountOptions>(Configuration);
            services.Configure<EventHubOptions>(Configuration);
            services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));

            if (HostingEnvironment.IsProduction() || Configuration["PersistentStores"] == "true")
            {
                UsePersistentStores(services);
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                UseInMemoryStores(services);
            }
        }

        private void UsePersistentStores(IServiceCollection services)
        {
            // COSMOS stores
            services.AddSingleton<IEntityStore<Amphora.Common.Models.Amphora>, CosmosAmphoraStore>();
            services.AddSingleton<IEntityStore<Amphora.Common.Models.Organisation>, CosmosOrganisationStore>();
            services.AddSingleton<IEntityStore<Amphora.Common.Models.OnboardingState>, CosmosOnboardingStateStore>();
            // consumed by some singletons
            services.AddSingleton<IEntityStore<Amphora.Common.Models.PermissionCollection>, CosmosPermissionCollectionStore>();

            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, Datum>, SignalEventHubDataStore>();
            // TODO (these are in memory)
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, AzBlobAmphoraDataStore>();

        }

        private static void UseInMemoryStores(IServiceCollection services)
        {
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, Datum>, InMemoryDataStore<Amphora.Common.Models.Amphora, Datum>>();

            // entity stores
            services.AddSingleton<IEntityStore<Amphora.Common.Models.Amphora>, InMemoryEntityStore<Amphora.Common.Models.Amphora>>();
            services.AddSingleton<IEntityStore<Organisation>, InMemoryEntityStore<Organisation>>();
            services.AddSingleton<IEntityStore<PermissionCollection>, InMemoryEntityStore<PermissionCollection>>();
            services.AddSingleton<IEntityStore<OnboardingState>, InMemoryEntityStore<OnboardingState>>();
        }


    }
}