using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Amphora.Api.Options;
using Amphora.Api.Services;
using Amphora.Api.StartupModules;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.Organisations;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Api.Services.Transactions;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;
using Amphora.Api.Converters;

namespace Amphora.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            this.AuthenticationModule = new AuthModule(configuration, env);
            this.IdentityModule = new IdentityModule(configuration, env);
            this.StorageModule = new StorageModule(configuration, env);
            this.GeoModule = new GeoModule(configuration, env);
            this.MarketModule = new MarketModule(configuration, env);
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        private readonly AuthModule AuthenticationModule;
        private readonly IdentityModule IdentityModule;
        private readonly StorageModule StorageModule;
        private readonly GeoModule GeoModule;
        private readonly MarketModule MarketModule;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            System.Console.WriteLine($"Hosting Environment Name is {HostingEnvironment.EnvironmentName}");
            if (HostingEnvironment.IsDevelopment())
            {
                System.Console.WriteLine("Is Development Environment");
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper("RunAs=Developer; DeveloperTool=AzureCli"));
            }
            else
            {
                services.AddSingleton<IAzureServiceTokenProvider>(new AzureServiceTokenProviderWrapper());
            }

            this.StorageModule.ConfigureServices(services);
            this.IdentityModule.ConfigureServices(services);
            this.AuthenticationModule.ConfigureServices(services);
            this.GeoModule.ConfigureServices(services);
            this.MarketModule.ConfigureServices(services);

            services.Configure<SendGridOptions>(Configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            services.AddScoped<ISignalService, SignalsService>();
            services.Configure<TsiOptions>(Configuration.GetSection("Tsi"));
            services.AddScoped<ITsiService, TsiService>();

            services.Configure<ChatOptions>(Configuration.GetSection("Chat"));
            services.Configure<FeedbackOptions>(Configuration.GetSection("Feedback"));

            services.Configure<CreateOptions>(Configuration.GetSection("Create"));

            // logical services
            services.AddTransient<IAmphoraeService, AmphoraeService>();
            services.AddTransient<IAmphoraFileService, AmphoraFileService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<IPurchaseService, PurchaseService>();

            services.Configure<FeatureFlagOptions>(Configuration.GetSection("FeatureFlags"));
            services.AddSingleton<FeatureFlagService>();


            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(typeof(Startup));
            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddMvc(opts =>
            {
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new TimespanConverter());
                options.SerializerSettings.Converters.Add(new TsiVariableConverter());
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddOpenApiDocument(document => // add OpenAPI v3 document
            {
                document.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {your JWT token}."
                });


                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                document.Description = "API for interacting with the Amphora Data platform.";
                document.Title = "Amphora Data Api";
                document.Version = "0.1.0";

            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            this.IdentityModule.Configure(app, env, mapper);
            this.StorageModule.Configure(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(settings =>
           {

           }); // serve Swagger UI
            app.UseReDoc(); // serve ReDoc UI


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Middleware.OrganisationCheckMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
