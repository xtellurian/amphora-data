using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

namespace Amphora.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            this.AuthenticationModule = new AuthModule(configuration, env);
            this.IdentityModule = new IdentityModule(configuration, env);
            this.StorageModule = new StorageModule(configuration, env);
            this.GeoModule = new GeoModule(configuration, env);
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        private readonly AuthModule AuthenticationModule;
        private readonly IdentityModule IdentityModule;
        private readonly StorageModule StorageModule;
        private readonly GeoModule GeoModule;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAzureServiceTokenProvider, AzureServiceTokenProviderWrapper>();

            this.StorageModule.ConfigureServices(services);
            this.IdentityModule.ConfigureServices(services);
            this.AuthenticationModule.ConfigureServices(services);
            this.GeoModule.ConfigureServices(services);

            services.AddTransient<IEmailSender, EmailSender>(); // todo 
            services.AddScoped<IMarketService, MarketService>();
            services.Configure<TsiOptions>(Configuration);
            services.AddScoped<ITsiService, RealTsiService>();
            services.Configure<CreateOptions>(Configuration.GetSection("Create"));
            services.AddTransient<IAmphoraeService, AmphoraeService>();
            services.AddTransient<IAmphoraFileService, AmphoraFileService>();
            
            services.AddTransient<IOnboardingService, OnboardingService>();


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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            }
            else
            {
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmphoraApi", Version = "v1" });
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
