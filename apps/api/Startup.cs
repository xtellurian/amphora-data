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
            if(HostingEnvironment.IsDevelopment())
            {
                UseInMemoryStores(services);
            }
            else if (HostingEnvironment.IsProduction())
            {
                UsePersistentStores(services);
            }
            

            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.Configure<TableStoreOptions>(Configuration);

        }
        private void UsePersistentStores(IServiceCollection services)
        {
            services.AddScoped<IEntityStore<Amphora.Common.Models.Amphora>, AzureTableAmphoraModelService>();
            // need to add the schema store here
        }
        private static void UseInMemoryStores(IServiceCollection services)
        {
            services.AddSingleton<IEntityStore<Amphora.Common.Models.Amphora>, InMemoryEntityStore<Amphora.Common.Models.Amphora>>();
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
