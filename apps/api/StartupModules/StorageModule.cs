using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Api.Services.Wrappers;
using Amphora.Api.Stores.AzureStorageAccount;
using Amphora.Api.Stores.EFCore;
using Amphora.Api.Stores.InMemory;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Activities;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Common.Options;
using Amphora.Common.Services.Azure;
using Amphora.Infrastructure.Database.EFCoreProviders;
using Amphora.Infrastructure.Extensions;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Services;
using Amphora.Infrastructure.Stores.Applications;
using Amphora.Infrastructure.Stores.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
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
            this.Configuration = configuration;
        }

        private bool IsUsingCosmos() => HostingEnvironment.IsProduction() || Configuration.IsPersistentStores();

        public IWebHostEnvironment HostingEnvironment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

            var connectionString = Configuration.GetSection("Storage")[nameof(AzureStorageAccountOptions.StorageConnectionString)];
            var kvUri = Configuration["kvUri"];
            if (HostingEnvironment.IsProduction() || Configuration.IsPersistentStores())
            {
                services.RegisterKeyVaultWithBlobDataProtection(connectionString, "key-container", "keys.xml", kvUri, "AmphoraData");
            }

            services.AddScoped<IEventHubSender, EventHubSender>();
            services.Configure<EventHubOptions>(Configuration.GetSection("TsiEventHub"));

            services.Configure<AzureStorageAccountOptions>(Configuration.GetSection("Storage"));
            services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));

            if (IsUsingCosmos())
            {
                var cosmosOptions = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmosOptions);
                services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));
                services.UseCosmos<AmphoraContext>(cosmosOptions);

                services.AddSingleton<IAmphoraBlobStore, AmphoraBlobStore>();
                services.AddSingleton<IBlobStore<OrganisationModel>, OrganisationBlobStore>();
                services.AddSingleton<IBlobCache, PlatformCacheBlobStore>();
                services.RegisterApplications(cosmosOptions, HostingEnvironment.IsDevelopment());
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                var amphoraSqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer:Amphora").Bind(amphoraSqlOptions);
                services.UseSqlServer<AmphoraContext>(amphoraSqlOptions);
                services.AddSingleton<IAmphoraBlobStore, InMemoryAmphoraBlobStore>();
                services.AddSingleton<IBlobStore<OrganisationModel>, InMemoryBlobStore<OrganisationModel>>();
                services.AddSingleton<IBlobCache, InMemoryBlobCache>();
                // applications
                var appsSqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer:Applications").Bind(appsSqlOptions);
                services.RegisterApplications(appsSqlOptions, HostingEnvironment.IsDevelopment());
            }

            services.AddTransient<CosmosInitialiser<AmphoraContext>>();
            // entity stores
            services.AddScoped<IEntityStore<AmphoraAccessControlModel>, AmphoraAccessControlsEFStore>();
            services.AddScoped<IEntityStore<AmphoraModel>, AmphoraeEFStore>();
            services.AddScoped<IEntityStore<ActivityModel>, ActivitiesEFStore>();
            services.AddScoped<IEntityStore<OrganisationModel>, OrganisationsEFStore>();
            services.AddScoped<IEntityStore<TermsOfUseModel>, TermsOfUseEFStore>();
            services.AddScoped<IEntityStore<DataRequestModel>, DataRequestsEFStore>();
            services.AddScoped<IEntityStore<PurchaseModel>, PurchaseEFStore>();
            services.AddScoped<IEntityStore<InvitationModel>, InvitationsEFStore>();
            services.AddScoped<IEntityStore<CommissionModel>, CommissionsEFStore>();
            services.AddScoped<IEntityStore<ApplicationUserDataModel>, ApplicationUserDataEFStore>();
            services.AddScoped<IEntityStore<ApplicationModel>, ApplicationModelEFStore>();

            // cache
            services.AddSingleton<ICache, InMemoryCache>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetService<CosmosInitialiser<AmphoraContext>>();
                initialiser.EnsureContainerCreated().ConfigureAwait(false);
                initialiser.EnableCosmosTimeToLive().ConfigureAwait(false);
                initialiser.LogInformationAsync().ConfigureAwait(false);
            }

            if (!IsUsingCosmos())
            {
                app.MigrateSql<AmphoraContext>();
            }
        }
    }
}