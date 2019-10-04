using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Amphora.Api.Options;
using Microsoft.OpenApi.Models;
using Amphora.Api.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Amphora.Api.StartupModules;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Azure;
using Amphora.Api.Services.Market;
using Amphora.Api.Services.Organisations;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Api.Services.Transactions;

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
            if(HostingEnvironment.IsDevelopment())
            {
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

            services.AddTransient<IEmailSender, EmailSender>(); // todo 
            
            services.AddScoped<ISignalService, SignalsService>();
            services.Configure<TsiOptions>(Configuration.GetSection("Tsi"));
            services.AddScoped<ITsiService, TsiService>();

            services.Configure<CreateOptions>(Configuration.GetSection("Create"));

            // logical services
            services.AddTransient<IAmphoraeService, AmphoraeService>();
            services.AddTransient<IAmphoraFileService, AmphoraFileService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<ITransactionService, TransactionService>();

            services.Configure<FeatureFlagOptions>(Configuration.GetSection("FeatureFlags"));
            services.AddSingleton<FeatureFlagService>();


            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());
            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddMvc(opts =>
                {
                    // this let's you work with the pages when in dev mode
                    opts.Filters.Add(new AllowAnonymousFilter());
                })
                //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            }
            else
            {
                services.AddMvc()
                    //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            }
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmphoraApi", Version = "v1" });
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AmphoraApi");
            });


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
