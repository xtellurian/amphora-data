using System.Net;
using System.Threading.Tasks;
using Amphora.Api.AspNet.Cors;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Applications;
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
using Amphora.Common.Services.Activities;
using Amphora.Common.Services.Azure;
using Amphora.Common.Services.Plans;
using Amphora.Infrastructure.Database.Cache;
using Amphora.Infrastructure.Extensions;
using Amphora.Infrastructure.Modules;
using Amphora.Infrastructure.Services;
using Amphora.Infrastructure.Services.Azure;
using Amphora.Workflow;
using AutoMapper;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json.Serialization;
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
            this.openApiModule = new OpenApiModule(configuration, env);
        }

        private readonly IdentityModule identityModule;
        private readonly StorageModule storageModule;
        private readonly GeoModule geoModule;
        private readonly DiscoverModule discoverModule;
        private readonly OpenApiModule openApiModule;

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
            this.openApiModule.ConfigureServices(services);
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
            services.AddTransient<ITermsOfUseService, TermsOfUseService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IActivityRunService, ActivityRunService>();
            services.AddTransient<IApplicationService, ApplicationService>();

            services.AddSingleton<IAmphoraGitHubIssueConnectorService, AmphoraGitHubIssueConnectorService>();

            services.AddMarkdown(); // Westwind.AspNetCore.Markdown
            services.AddAllPollyHttpClients();

            // add the rate limiting middleware
            services.UseRateLimitingByIpAddress(Configuration);

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

            // CORS for applications
            services.AddCors();
            services.AddScoped<ICorsPolicyProvider, ConnectedCorsPolicyProvider>();

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
                options.LoginPath = "/Profiles/Account/Login";
                options.LogoutPath = "/Profiles/Account/Logout";
                options.AccessDeniedPath = "/AccessDenied";
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // add workflows
            services.RegisterWorkflows();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            // write to console whether we are running the SPA
            var isUsingSpa = Configuration.GetSection("FeatureManagement")["Spa"];
            System.Console.WriteLine($"SPA Enabled: {isUsingSpa}");

            app.UseForFeature(nameof(ApiFeatureFlags.Spa), appBuilder =>
            {
                CommonPipeline(appBuilder, env, mapper);
                appBuilder.UseSpaStaticFiles();
                appBuilder.UseEndpoints(endpoints =>
                {
                    // endpoints.MapRazorPages(); // disable razor pages on SPA mode
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                })
                .UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            });
            CommonPipeline(app, env, mapper);
            // default application
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/Quickstart");
                    return Task.CompletedTask;
                });
            });
        }

        // The shared config pipeline across both SPA and Raor
        private void CommonPipeline(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            ConfigureSharedPipeline(app);
            // if (env.IsDevelopment())
            // {
            //     // do this for integration tests where the remote IP doesn't exist
            //     var fakeIpAddress = IPAddress.Parse("127.168.1.32");
            //     app.Use(async (context, next) =>
            //     {
            //         context.Connection.RemoteIpAddress = fakeIpAddress;
            //         await next.Invoke();
            //     });
            // }

            app.UseAzureAppConfiguration();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.identityModule.Configure(app, env, mapper);
            this.storageModule.Configure(app, env);
            this.openApiModule.Configure(app);
            app.UseRouting();
            app.UseCors("test"); // needs to match policy name above

            app.UseMarkdown(); // Westwind.AspNetCore.Markdown
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Middleware.UserDataMiddleware>();
            app.UseMiddleware<Middleware.PlanSelectorMiddleware>();
            app.UseMiddleware<Middleware.ClientTrackerMiddleware>();
        }
    }
}
