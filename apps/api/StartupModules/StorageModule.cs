using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Stores;
using Amphora.Api.Stores.AzureStorageAccount;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amphora.Api.DbContexts;
using Microsoft.EntityFrameworkCore;
using Amphora.Api.Stores.EFCore;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Amphora.Common.Models.Platform;

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
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

            var connectionString = Configuration[nameof(AzureStorageAccountOptions.StorageConnectionString)];
            var kvUri = Configuration["kvUri"];
            if (CloudStorageAccount.TryParse(connectionString, out var storageAccount) && !string.IsNullOrEmpty(kvUri))
            {
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference("key-container");
                container.CreateIfNotExists();
                var keyIdentifier = $"{kvUri}keys/dataprotection/";
                services.AddDataProtection()
                    .PersistKeysToAzureBlobStorage(container, "keys.xml")
                    .ProtectKeysWithAzureKeyVault(keyVaultClient, keyIdentifier);

            }
            else
            {
                Console.WriteLine("Not Configuring DataProtection");
            }

            services.Configure<AzureStorageAccountOptions>(Configuration);
            services.Configure<EventHubOptions>(Configuration.GetSection("TsiEventHub"));
            services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));

            if (HostingEnvironment.IsProduction() || Configuration["PersistentStores"] == "true")
            {
                var cosmos = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmos);
                services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));
                services.AddDbContext<AmphoraContext>(options =>
                {
                    options.UseCosmos(cosmos.Endpoint, cosmos.PrimaryKey, cosmos.Database);
                    options.UseLazyLoadingProxies();
                });

                services.AddSingleton<IBlobStore<AmphoraModel>, AmphoraBlobStore>();
                services.AddSingleton<IBlobStore<OrganisationModel>, OrganisationBlobStore>();
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<AmphoraContext>(options =>
                {
                    options.UseInMemoryDatabase("Amphora");
                    options.UseLazyLoadingProxies();
                });

                services.AddSingleton<IBlobStore<AmphoraModel>, InMemoryBlobStore<AmphoraModel>>();
                services.AddSingleton<IBlobStore<OrganisationModel>, InMemoryBlobStore<OrganisationModel>>();
            }

            services.AddTransient<CosmosInitialiser>();
            // entity stores
            services.AddScoped<IEntityStore<AmphoraModel>, AmphoraeEFStore>();
            services.AddScoped<IEntityStore<OrganisationModel>, OrganisationsEFStore>();
            services.AddScoped<IEntityStore<PurchaseModel>, PurchaseEFStore>();
            services.AddScoped<IEntityStore<InvitationModel>, InvitationsEFStore>();
            // services.AddScoped<IEntityStore<UserModel>, UserEFStore>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetService<AmphoraContext>())
            {
                context.Database.EnsureCreated();
            }
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetService<CosmosInitialiser>();
                initialiser.EnableCosmosTimeToLive().ConfigureAwait(false);
            }
        }
    }
}