using System.Linq;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.DataRequests;
using Amphora.Api.Services.GitHub;
using Amphora.Api.Services.InMemory;
using Amphora.Api.Services.Organisations;
using Amphora.Api.Services.Purchases;
using Amphora.Api.StartupModules;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Services.Access;
using Amphora.Common.Services.Azure;
using Amphora.Common.Services.Plans;
using Amphora.Infrastructure.Modules;
using Amphora.Infrastructure.Services;
using AutoMapper;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using Westwind.AspNetCore.Markdown;

namespace Amphora.Api
{
    public class Startup : SharedUI.SharedStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            this.identityModule = new IdentityModule(configuration, env);
            this.storageModule = new StorageModule(configuration, env);
            this.geoModule = new GeoModule(configuration, env);
            this.discoverModule = new DiscoverModule(configuration, env);
        }

        private readonly IdentityModule identityModule;
        private readonly StorageModule storageModule;
        private readonly GeoModule geoModule;
        private readonly DiscoverModule discoverModule;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSharedServices(services);
            ApiConfiguration.RegisterOptions(Configuration, services, HostingEnvironment.IsDevelopment());

            System.Console.WriteLine($"Persistent Stores: {Configuration.IsPersistentStores()}");

            this.storageModule.ConfigureServices(services);
            this.identityModule.ConfigureServices(services);
            this.geoModule.ConfigureServices(services);
            this.discoverModule.ConfigureServices(services);
            services.RegisterEventsModule(Configuration, HostingEnvironment.IsProduction());

            // feature management
            services.AddFeatureManagement().AddFeatureFilter<PercentageFilter>();

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper("RunAs=Developer; DeveloperTool=AzureCli"));
            }
            else
            {
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper());
            }

            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IAmphoraeTextAnalysisService, AmphoraeTextAnalysisService>();
            services.AddScoped<ISignalService, SignalsService>();

            if (Configuration.IsPersistentStores() || HostingEnvironment.IsProduction())
            {
                services.AddScoped<ITsiService, TsiService>();
            }
            else
            {
                services.AddScoped<ITsiService, InMemoryTsiService>();
            }

            // permissioned stores
            services.AddTransient<IPermissionedEntityStore<DataRequestModel>, DataRequestService>();
            // logical services
            services.AddTransient<IAmphoraeService, AmphoraeService>();
            services.AddTransient<IAmphoraFileService, AmphoraFileService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<ICommissionTrackingService, CommissionTrackingService>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<IInvoiceFileService, InvoiceFileService>();
            services.AddTransient<IQualityEstimatorService, QualityEstimatorService>();
            services.AddSingleton<IDateTimeProvider, Common.Services.Timing.DateTimeProvider>();
            services.AddTransient<IAccessControlService, AccessControlService>();
            services.AddTransient<IPlanLimitService, PlanLimitService>();

            services.AddSingleton<IAmphoraGitHubIssueConnectorService, AmphoraGitHubIssueConnectorService>();

            services.AddMarkdown(); // Westwind.AspNetCore.Markdown
            services.AddHttpClient();

            // The following will configure the channel to use the given folder to temporarily
            // store telemetry items during network or Application Insights server issues.
            // User should ensure that the given folder already exists
            // and that the application has read/write permissions.
            services.AddSingleton(typeof(ITelemetryChannel),
                                    new ServerTelemetryChannel() { StorageFolder = "/tmp/appinsights" });
            services.AddApplicationInsightsTelemetry();

            services.AddAutoMapper(typeof(Startup));
            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddMvc(opts =>
            {
            })
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Profiles", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Profiles", "/Account/Logout");
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new Iso8601TimeSpanConverter());
                options.SerializerSettings.Converters.Add(new PolymorphicSerializeJsonConverter<Variable>("kind"));
                options.SerializerSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Variable>("kind"));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Profiles/Account/Login";
                options.LogoutPath = $"/Profiles/Account/Logout";
                options.AccessDeniedPath = $"/Profiles/Account/AccessDenied"; // TODO: create access denided oasge
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddOpenApiDocument(document => // add OpenAPI v3 document
            {
                document.DocumentName = "v1";
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AmphoraDataApiVersionOperationProcessor());

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                document.Description = "API for interacting with the Amphora Data platform.";
                document.Title = "Amphora Data Api";
                document.Version = ApiVersion.CurrentVersion.ToSemver();
            });

            services.AddSwaggerDocument(document => // add a Swagger (OpenAPI v2) doc
            {
                document.DocumentName = "v2";
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AmphoraDataApiVersionOperationProcessor());

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                document.Description = "API for interacting with the Amphora Data platform.";
                document.Title = "Amphora Data Api";
                document.Version = ApiVersion.CurrentVersion.ToSemver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            ConfigureSharedPipeline(app);

            app.UseAzureAppConfiguration();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.identityModule.Configure(app, env, mapper);
            this.storageModule.Configure(app, env);

            app.UseRouting();

            app.UseMarkdown(); // Westwind.AspNetCore.Markdown
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(settings => { }); // serve Swagger UI
            // app.UseReDoc(); // serve ReDoc UI

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Middleware.UserDataMiddleware>();
            app.UseMiddleware<Middleware.PlanSelectorMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
