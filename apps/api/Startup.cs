using Amphora.Api.Contracts;
using AutoMapper;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Amphora.Api.Stores;
using Amphora.Api.Options;
using api.Store;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Amphora.Api.Services;
using Amphora.Api.Models;

namespace Amphora.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            if (HostingEnvironment.IsProduction() || Configuration["PersistantStores"] == "true")
            {
                UsePersistentStores(services);
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                UseInMemoryStores(services);
            }

            services.AddScoped<ITsiService, RealTsiService>();

            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());
            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            ConfigureOptions(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmphoraApi", Version = "v1" });
            });

        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<TableStoreOptions>(Configuration);
            services.Configure<EventHubOptions>(Configuration);
            services.Configure<TsiOptions>(Configuration);
            services.Configure<EntityTableStoreOptions<AmphoraTableEntity>>(p =>
            {
                p.TableName = "amphorae";
            });
            services.Configure<EntityTableStoreOptions<TemporaTableEntity>>(p =>
            {
                p.TableName = "temporae";
            });
        }

        private void UsePersistentStores(IServiceCollection services)
        {
            // org entity stores
            services.AddScoped<IOrgEntityStore<Amphora.Common.Models.Amphora>, AzTableOrgEntityStore<Amphora.Common.Models.Amphora, AmphoraTableEntity>>();
            services.AddScoped<IOrgEntityStore<Amphora.Common.Models.Tempora>, AzTableOrgEntityStore<Amphora.Common.Models.Tempora, TemporaTableEntity>>();
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, TemporaEventHubDataStore>();
            // TODO (these are in memory)
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();

            // schemas 
            // using in memory for now (not implemented properly)
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();

        }
        private static void UseInMemoryStores(IServiceCollection services)
        {
            services.AddSingleton<IOrgEntityStore<Amphora.Common.Models.Amphora>, InMemoryOrgEntityStore<Amphora.Common.Models.Amphora>>();
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, InMemoryDataStore<Amphora.Common.Models.Tempora, JObject>>();

            //temporae
            services.AddSingleton<IOrgEntityStore<Amphora.Common.Models.Tempora>, InMemoryOrgEntityStore<Amphora.Common.Models.Tempora>>();

            // this isnt actually in memory :()
            services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, TemporaEventHubDataStore>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
