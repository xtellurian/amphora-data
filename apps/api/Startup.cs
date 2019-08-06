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
using System;
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
            // temporary adding this here
            services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, TemporaDataStore>();
            if (HostingEnvironment.IsProduction() || Configuration["PersistantStores"] == "true")
            {
                UsePersistentStores(services);
            }
            else if (HostingEnvironment.IsDevelopment())
            {
                UseInMemoryStores(services);
            }

            services.AddScoped<ITsiService, TsiService>();

            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.Configure<TableStoreOptions>(Configuration);
            services.Configure<EventHubOptions>(Configuration);
            services.Configure<TsiOptions>(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmphoraApi", Version = "v1" });
            });

        }
        private void UsePersistentStores(IServiceCollection services)
        {
            services.AddScoped<IOrgEntityStore<Amphora.Common.Models.Amphora>, AzTableOrgEntityStore<Amphora.Common.Models.Amphora, AmphoraTableEntity>>();
            services.AddScoped<IOrgEntityStore<Amphora.Common.Models.Tempora>, AzTableOrgEntityStore<Amphora.Common.Models.Tempora, TemporaTableEntity>>();
            // using in memory for now (not implemented)
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();

        }
        private static void UseInMemoryStores(IServiceCollection services)
        {
            services.AddSingleton<IOrgEntityStore<Amphora.Common.Models.Amphora>, InMemoryDataEntityStore<Amphora.Common.Models.Amphora>>();
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();

            //temporae
            services.AddSingleton<IOrgEntityStore<Amphora.Common.Models.Tempora>, InMemoryDataEntityStore<Amphora.Common.Models.Tempora>>();

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
