using Amphora.Api.Services.Events;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Identity.Contracts;
using Amphora.Identity.EntityFramework;
using Amphora.Identity.Extensions;
using Amphora.Identity.Models;
using Amphora.Identity.Services;
using Amphora.Identity.Stores;
using Amphora.Infrastructure.Database.EFCoreProviders;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Services;
using Amphora.Infrastructure.Stores.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

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

            System.Console.WriteLine($"Persistent Stores: {Configuration.IsPersistentStores()}");

            // Data Protection
            var connectionString = Configuration.GetSection("Storage")[nameof(AzureStorageAccountOptions.StorageConnectionString)];
            var kvUri = Configuration["kvUri"];
            if (HostingEnvironment.IsProduction() || Configuration.IsPersistentStores())
            {
                services.RegisterKeyVaultWithBlobDataProtection(connectionString, "key-container", "keys.xml", kvUri, "AmphoraIdentity");
            }

            services.Configure<ExternalServices>(Configuration.GetSection("ExternalServices"));
            var externalServices = new ExternalServices();
            Configuration.GetSection("ExternalServices").Bind(externalServices);

            var mvcClientSecret = Configuration["MvcClientSecret"];

            if (IsUsingCosmos())
            {
                var cosmosOptions = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmosOptions);
                services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));
                services.UseCosmos<IdentityContext>(cosmosOptions);
            }
            else if (IsUsingSql())
            {
                var sqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer").Bind(sqlOptions);
                services.UseSqlServer<IdentityContext>(sqlOptions);
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                services.UseInMemory<IdentityContext>();
            }
            else
            {
                throw new System.ApplicationException("No DB Context Configured");
            }

            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Account/Logout";
                })
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients(externalServices, EnvironmentInfo, mvcClientSecret))
                .AddProfileService<IdentityProfileService>()
                .AddAspNetIdentity<ApplicationUser>();

            if (HostingEnvironment.IsProduction() || Configuration.IsPersistentStores())
            {
                builder.AddSigningCredentialFromAzureKeyVault(kvUri, "identityCert", 6);
            }
            else
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }

            services.AddScoped<IUserService, UserService>();
            if (Configuration.IsPersistentStores() || HostingEnvironment.IsProduction())
            {
                services.Configure<AzureEventGridTopicOptions>("AppTopic", Configuration.GetSection("EventGrid").GetSection("AppTopic"));
                services.AddTransient<IEventPublisher, EventGridService>();
            }
            else
            {
                services.AddTransient<IEventPublisher, LoggingEventPublisher>();
            }

            services.AddTransient<CosmosInitialiser<IdentityContext>>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.AddScoped<IEntityStore<ApplicationUser>, UsersEFStore>();
            services.AddScoped<IIdentityService, IdentityServerService>();

            services.Configure<SendGridOptions>(Configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            services.AddAuthentication();

            services.Configure<Amphora.Common.Models.Host.HostOptions>(Configuration.GetSection("Host"));

            services.AddMvc(opts =>
            {
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Login";
                options.LogoutPath = $"/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied"; // TODO: create access denided oasge
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // temp
            IdentityModelEventSource.ShowPII = true;
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
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetService<CosmosInitialiser<IdentityContext>>();
                initialiser.EnsureContainerCreated().ConfigureAwait(false);
                initialiser.EnableCosmosTimeToLive().ConfigureAwait(false);
                initialiser.LogInformationAsync().ConfigureAwait(false);
            }
        }
    }
}