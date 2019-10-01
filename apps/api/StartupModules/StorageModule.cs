using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Stores;
using Amphora.Api.Stores.AzureStorageAccount;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Transactions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amphora.Api.DbContexts;
using Microsoft.EntityFrameworkCore;
using Amphora.Api.Stores.EFCore;
using Microsoft.AspNetCore.Builder;

namespace Amphora.Api.StartupModules
{
    public class StorageModule
    {
        public StorageModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IWebHostEnvironment HostingEnvironment { get; }

        private readonly IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureStorageAccountOptions>(Configuration);
            services.Configure<EventHubOptions>(Configuration.GetSection("TsiEventHub"));
            services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));

            if (HostingEnvironment.IsProduction() || Configuration["PersistentStores"] == "true")
            {
                var cosmos = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmos);
                services.AddDbContext<AmphoraContext>(options => options.UseCosmos(cosmos.Endpoint, cosmos.PrimaryKey, cosmos.Database));

                services.AddSingleton<IBlobStore<AmphoraModel>, AmphoraBlobStore>();
                services.AddSingleton<IBlobStore<OrganisationModel>, OrganisationBlobStore>();
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<AmphoraContext>(options => options.UseInMemoryDatabase("Amphora"));

                services.AddSingleton<IBlobStore<AmphoraModel>, InMemoryBlobStore<AmphoraModel>>();
                services.AddSingleton<IBlobStore<OrganisationModel>, InMemoryBlobStore<OrganisationModel>>();
            }
            // entity stores
            services.AddScoped<IEntityStore<AmphoraModel>, AmphoraeEFStore>();
            services.AddScoped<IEntityStore<OrganisationModel>, OrganisationsEFStore>();
            services.AddScoped<IEntityStore<TransactionModel>, TransactionEFStore>();
            // services.AddScoped<IEntityStore<UserModel>, UserEFStore>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetService<AmphoraContext>())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}