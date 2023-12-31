﻿using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Common.Services.Emails;
using Amphora.Identity.Contracts;
using Amphora.Identity.EntityFramework;
using Amphora.Identity.Extensions;
using Amphora.Identity.IdentityConfig;
using Amphora.Identity.Models;
using Amphora.Identity.Services;
using Amphora.Identity.Stores;
using Amphora.Identity.Stores.IdentityServer;
using Amphora.Infrastructure.Database.Contexts;
using Amphora.Infrastructure.Database.EFCoreProviders;
using Amphora.Infrastructure.Extensions;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Modules;
using Amphora.Infrastructure.Services;
using Amphora.Infrastructure.Services.Emails;
using Amphora.Infrastructure.Stores.EFCore;
using IdentityServer4.Services;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Identity
{
    public class Startup : SharedUI.SharedStartup
    {
        private bool IsUsingCosmos() => HostingEnvironment.IsProduction() || Configuration.IsPersistentStores();
        private bool IsUsingSql() => !IsUsingCosmos() && Configuration["sql"] == "true";

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            HostingEnvironment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSharedServices(services);
            IdentityConfiguration.RegisterOptions(Configuration, services, HostingEnvironment.IsDevelopment());
            System.Console.WriteLine($"Persistent Stores: {Configuration.IsPersistentStores()}");

            services.RegisterEventsModule(Configuration, HostingEnvironment.IsProduction());

            // Data Protection
            var connectionString = Configuration.GetSection("Storage")[nameof(AzureStorageAccountOptions.StorageConnectionString)];
            var kvUri = Configuration["kvUri"];
            if (HostingEnvironment.IsProduction() || Configuration.IsPersistentStores())
            {
                services.RegisterKeyVaultWithBlobDataProtection(connectionString, "key-container", "keys.xml", kvUri, "AmphoraIdentity");
            }

            var externalServices = new ExternalServices();
            Configuration.GetSection("ExternalServices").Bind(externalServices);

            var mvcClientSecret = Configuration["MvcClientSecret"];
            services.Configure<MvcOptions>(Configuration);

            // The following will configure the channel to use the given folder to temporarily
            // store telemetry items during network or Application Insights server issues.
            // User should ensure that the given folder already exists
            // and that the application has read/write permissions.
            services.AddSingleton(typeof(ITelemetryChannel),
                                    new ServerTelemetryChannel() { StorageFolder = "/tmp/appinsights" });
            services.AddApplicationInsightsTelemetry();
            services.Configure<OAuthClientSecret>(Configuration);
            if (IsUsingCosmos())
            {
                var cosmosOptions = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmosOptions);
                services.UseCosmos<IdentityContext>(cosmosOptions);
                services.RegisterApplications(cosmosOptions, HostingEnvironment.IsDevelopment());
            }
            else if (IsUsingSql())
            {
                var identitySqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer:Identity").Bind(identitySqlOptions);
                services.UseSqlServer<IdentityContext>(identitySqlOptions);
                // apps context for sql
                var applicationsSqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer:Applications").Bind(applicationsSqlOptions);
                services.RegisterApplications(applicationsSqlOptions, HostingEnvironment.IsDevelopment());
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                services.UseInMemory<IdentityContext>();
                services.RegisterApplications(HostingEnvironment.IsDevelopment()); // use in memory applications store
            }
            else
            {
                throw new System.ApplicationException("No DB Context Configured");
            }

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
            })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            IIdentityServerConfig config;
            if (HostingEnvironment.IsDevelopment())
            {
                config = new DevelopmentConfig(mvcClientSecret);
            }
            else
            {
                config = new ProductionConfig(externalServices, EnvironmentInfo, mvcClientSecret);
            }

            // this should be added before callsing AddAspNetIdentity
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();

            // add this to the container so it can be consumed in InMemoryResourceStore
            services.AddSingleton<IIdentityServerConfig>(config);
            services.AddTransient<IAmphoraClaimsService, AmphoraClaimsService>();
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/Login";
                    options.UserInteraction.LogoutUrl = "/Account/Logout";
                })
                .AddResourceStore<InMemoryResourceStore>()
                .AddClientStore<ConnectedClientStore>()
                .AddProfileService<IdentityProfileService>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddAspNetIdentity<ApplicationUser>();

            if (HostingEnvironment.IsProduction() || Configuration.IsPersistentStores())
            {
                var certName = "identityCert";
                System.Console.WriteLine($"Using certificate {certName} from KeyVault as Signing Credentials");
                builder.AddSigningCredentialFromAzureKeyVault(kvUri, certName, 6);
            }
            else
            {
                // not recommended for production - you need to store your key material somewhere secure
                System.Console.WriteLine("WARNING: Using Developer Signing Credentials");
                builder.AddDeveloperSigningCredential();
            }

            services.AddScoped<IUserService, UserService>();

            services.AddTransient<CosmosInitialiser<IdentityContext>>();

            services.AddScoped<IEntityStore<ApplicationUser>, UsersEFStore>();
            services.AddScoped<IIdentityService, IdentityServerService>();
            services.AddScoped<IEventSink, IdentityServerEventConnectorService>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IEmailGenerator, EmailGenerator>();

            services.AddAuthentication();

            services.AddMvc(opts =>
            {
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.LoginPath = "/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/AccessDenied";
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddScoped<ICorsPolicyService, ConnectedCorsPolicyService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            ConfigureSharedPipeline(app);

            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

            if (IsUsingSql())
            {
                app.MigrateSql<IdentityContext>();
                app.MigrateSql<ApplicationsContext>();
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                InitCosmosContext<IdentityContext>(scope, true);
                InitCosmosContext<ApplicationsContext>(scope, false);
            }
        }

        private static void InitCosmosContext<T>(IServiceScope scope, bool isTtl = false) where T : DbContext
        {
            var initialiser = scope.ServiceProvider.GetService<CosmosInitialiser<T>>();
            if (initialiser != null)
            {
                initialiser.EnsureContainerCreated().ConfigureAwait(false);
                if (isTtl)
                {
                    initialiser.EnableCosmosTimeToLive().ConfigureAwait(false);
                }

                initialiser.LogInformationAsync().ConfigureAwait(false);
            }
            else
            {
                System.Console.WriteLine("WARNING: Cosmos Initialiser was null");
            }
        }
    }
}