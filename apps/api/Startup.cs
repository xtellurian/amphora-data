using System.Linq;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.DataRequests;
using Amphora.Api.Services.Events;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Api.Services.GitHub;
using Amphora.Api.Services.Organisations;
using Amphora.Api.Services.Purchases;
using Amphora.Api.StartupModules;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.GitHub;
using Amphora.Common.Models.Options;
using Amphora.Common.Options;
using Amphora.Common.Services.Azure;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            System.Console.WriteLine($"Persistent Stores: {Configuration.IsPersistentStores()}");

            this.storageModule.ConfigureServices(services);
            this.identityModule.ConfigureServices(services);
            this.geoModule.ConfigureServices(services);
            this.discoverModule.ConfigureServices(services);

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper("RunAs=Developer; DeveloperTool=AzureCli"));
            }
            else
            {
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper());
            }

            services.Configure<Amphora.Common.Models.Host.HostOptions>(Configuration.GetSection("Host"));
            AmphoraHost.SetHost(Configuration.GetSection("Host")["MainHost"]);

            services.Configure<SignalOptions>(Configuration.GetSection("Signals"));

            services.Configure<SendGridOptions>(Configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IAmphoraeTextAnalysisService, AmphoraeTextAnalysisService>();

            services.AddScoped<ISignalService, SignalsService>();
            services.Configure<TsiOptions>(Configuration.GetSection("Tsi"));
            services.AddScoped<ITsiService, TsiService>();

            services.Configure<ChatOptions>(Configuration.GetSection("Chat"));
            services.Configure<FeedbackOptions>(Configuration.GetSection("Feedback"));

            services.Configure<CreateOptions>(Configuration.GetSection("Create"));

            services.Configure<AmphoraManagementOptions>(Configuration.GetSection("AmphoraManagement"));
            if (Configuration.IsPersistentStores() || HostingEnvironment.IsProduction())
            {
                services.Configure<AzureEventGridTopicOptions>("AppTopic", Configuration.GetSection("EventGrid").GetSection("AppTopic"));
                services.AddTransient<IEventPublisher, EventGridService>();
            }
            else
            {
                services.AddTransient<IEventPublisher, LoggingEventPublisher>();
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
            services.AddTransient<IRestrictionService, RestrictionService>();

            // external services
            services.Configure<GitHubConfiguration>(Configuration.GetSection("GitHubOptions"));
            services.AddSingleton<IAmphoraGitHubIssueConnectorService, AmphoraGitHubIssueConnectorService>();

            services.Configure<FeatureFlagOptions>(Configuration.GetSection("FeatureFlags"));
            services.AddSingleton<FeatureFlagService>();

            services.AddMarkdown(); // Westwind.AspNetCore.Markdown

            services.AddHttpClient();
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

            if (HostingEnvironment.IsProduction())
            {
                services.AddHttpsRedirection(options => options.HttpsPort = 443);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            ConfigureSharedPipeline(app);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            this.identityModule.Configure(app, env, mapper);
            this.storageModule.Configure(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Home/StatusCode?code={0}");
                app.UseExceptionHandler("/Home/Error");

                // check if this is set, if yes, then don't disable (i.e. opposite of IfEnabled)
                if (!string.IsNullOrEmpty(Configuration["DisableHsts"]))
                {
                    System.Console.WriteLine("Enabling HSTS");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                else
                {
                    System.Console.WriteLine("HSTS is not enabled");
                }
            }

            app.UseRouting();
            if (!HostingEnvironment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseMarkdown(); // Westwind.AspNetCore.Markdown
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(settings => { }); // serve Swagger UI
            // app.UseReDoc(); // serve ReDoc UI

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Middleware.OrganisationCheckMiddleware>();
            app.UseMiddleware<Middleware.UserDataMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
