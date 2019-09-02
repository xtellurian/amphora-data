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
            services.AddScoped<IEntityStore<Amphora.Common.Models.Amphora>, CosmosAmphoraStore>();
            services.AddScoped<IEntityStore<Amphora.Common.Models.Organisation>, CosmosOrganisationStore>();
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, Datum>, SignalEventHubDataStore>();
            // TODO (these are in memory)
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, AzBlobAmphoraDataStore>();
            

        }

        private static void UseInMemoryStores(IServiceCollection services)
        {
            services.AddSingleton<IEntityStore<Amphora.Common.Models.Amphora>, InMemoryEntityStore<Amphora.Common.Models.Amphora>>();
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, Datum>, InMemoryDataStore<Amphora.Common.Models.Amphora, Datum>>();

            // orgs
            services.AddSingleton<IEntityStore<Organisation>, InMemoryEntityStore<Organisation>>();
        }


    }
}