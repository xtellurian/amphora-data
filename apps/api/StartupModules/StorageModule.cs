using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Api.Stores;
using Amphora.Api.Stores.AzureStorageAccount;
using Amphora.Api.Stores.Cosmos;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
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
            services.Configure<EventHubOptions>(Configuration.GetSection("TsiEventHub"));
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
            services.AddSingleton<IEntityStore<Amphora.Common.Models.AmphoraModel>, CosmosAmphoraStore>();
            services.AddSingleton<IEntityStore<OrganisationModel>, CosmosOrganisationStore>();
            // consumed by some singletons
            services.AddSingleton<IEntityStore<PermissionModel>, CosmosPermissionStore>();

            // data stores
            services.AddSingleton<IBlobStore<Amphora.Common.Models.AmphoraModel>, AmphoraBlobStore>();
            services.AddSingleton<IBlobStore<OrganisationModel>, OrganisationBlobStore>();

        }

        private static void UseInMemoryStores(IServiceCollection services)
        {
            // data stores
            services.AddSingleton<IBlobStore<AmphoraModel>, InMemoryBlobStore<AmphoraModel>>();
            services.AddSingleton<IBlobStore<OrganisationModel>, InMemoryBlobStore<OrganisationModel>>();

            // entity stores
            services.AddSingleton<IEntityStore<Amphora.Common.Models.AmphoraModel>, InMemoryEntityStore<Amphora.Common.Models.AmphoraModel>>();
            services.AddSingleton<IEntityStore<OrganisationModel>, InMemoryEntityStore<OrganisationModel>>();
            services.AddSingleton<IEntityStore<PermissionModel>, InMemoryEntityStore<PermissionModel>>();
        }


    }
}