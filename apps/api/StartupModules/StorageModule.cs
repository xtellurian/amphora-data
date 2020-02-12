using System;
using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Api.Services.Wrappers;
using Amphora.Api.Stores;
using Amphora.Api.Stores.AzureStorageAccount;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Common.Options;
using Amphora.Common.Services.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Api.StartupModules
{
    public class StorageModule
    {
        public StorageModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.configuration = configuration;
        }

        public IWebHostEnvironment HostingEnvironment { get; }

        private readonly IConfiguration configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

            var connectionString = configuration.GetSection("Storage")[nameof(AzureStorageAccountOptions.StorageConnectionString)];
            var kvUri = configuration["kvUri"];
            if (CloudStorageAccount.TryParse(connectionString, out var storageAccount) && !string.IsNullOrEmpty(kvUri))
            {
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference("key-container");
                container.CreateIfNotExists();
                var keyIdentifier = $"{kvUri}keys/dataprotection/";
                services.AddDataProtection()
                    .SetApplicationName("AmphoraData")
                    .PersistKeysToAzureBlobStorage(container, "keys.xml")
                    .ProtectKeysWithAzureKeyVault(keyVaultClient, keyIdentifier);
            }
            else
            {
                Console.WriteLine("Not Configuring DataProtection");
            }

            services.AddScoped<IEventHubSender, EventHubSender>();

            services.Configure<AzureStorageAccountOptions>(configuration.GetSection("Storage"));
            services.Configure<EventHubOptions>(configuration.GetSection("TsiEventHub"));
            services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));

            if (HostingEnvironment.IsProduction() || configuration.IsPersistentStores())
            {
                var cosmosOptions = new CosmosOptions();
                configuration.GetSection("Cosmos").Bind(cosmosOptions);
                services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));
                services.AddDbContext<AmphoraContext>(options =>
                {
                    options.UseCosmos(cosmosOptions.Endpoint, cosmosOptions.PrimaryKey, cosmosOptions.Database);
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
            services.AddScoped<IEntityStore<DataRequestModel>, DataRequestsEFStore>();
            services.AddScoped<IEntityStore<PurchaseModel>, PurchaseEFStore>();
            services.AddScoped<IEntityStore<InvitationModel>, InvitationsEFStore>();
            services.AddScoped<IEntityStore<CommissionModel>, CommissionsEFStore>();
            // services.AddScoped<IEntityStore<SignalModel>, SignalsEFStore>();
            services.AddScoped<IEntityStore<ApplicationUser>, ApplicationUserStore>();

            // cache
            services.AddSingleton<ICache, InMemoryCache>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetService<CosmosInitialiser>();
                initialiser.EnsureContainerCreated().ConfigureAwait(false);
                initialiser.EnableCosmosTimeToLive().ConfigureAwait(false);
            }
        }
    }
}