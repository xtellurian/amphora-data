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
using Amphora.Api.Data;
using System;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;

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
            // wrap the user services


            SetupUserIdentities(services);
            SetupMarket(services);
            SetupToDoServices(services);

            services.AddScoped<ITsiService, RealTsiService>();
            services.AddScoped<IAzureServiceTokenProvider, AzureServiceTokenProviderWrapper>();

            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(System.AppDomain.CurrentDomain.GetAssemblies());
            // Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddMvc(opts =>
                {
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


            ConfigureOptions(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmphoraApi", Version = "v1" });
            });

        }

        private void SetupMarket(IServiceCollection services)
        {
            services.AddScoped<IMarketService, MarketService>();
        }

        private void SetupToDoServices(IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
        }

        private void SetupUserIdentities(IServiceCollection services)
        {
            services.AddScoped<ISignInManager<ApplicationUser>, SignInManagerWrapper<ApplicationUser>>();
            services.AddScoped<IUserManager<ApplicationUser>, UserManagerWrapper<ApplicationUser>>();
            if (HostingEnvironment.IsProduction())
            {
                Console.WriteLine("Disabling Dev SignIn for production");
                Models.Development.DevSignInResult.Disabled = true;
            }
            if (!string.IsNullOrEmpty(Configuration["StorageConnectionString"]))
            {

                System.Console.WriteLine("Setting Up User Identities");
                services.Configure<CookiePolicyOptions>(options =>
                {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });
                services.AddIdentity<ApplicationUser, ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityRole>((options) =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddAzureTableStoresV2<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
                    {
                        IdentityConfiguration idconfig = new IdentityConfiguration();
                        idconfig.TablePrefix = Configuration.GetSection("IdentityConfiguration:TablePrefix").Value;
                        idconfig.StorageConnectionString = Configuration.GetSection("StorageConnectionString").Value;
                        idconfig.LocationMode = Configuration.GetSection("IdentityConfiguration:LocationMode").Value;
                        idconfig.IndexTableName = Configuration.GetSection("IdentityConfiguration:IndexTableName").Value; // default: AspNetIndex
                    idconfig.RoleTableName = Configuration.GetSection("IdentityConfiguration:RoleTableName").Value;   // default: AspNetRoles
                    idconfig.UserTableName = Configuration.GetSection("IdentityConfiguration:UserTableName").Value;   // default: AspNetUsers
                    return idconfig;
                    }))
                    .AddDefaultTokenProviders()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .CreateAzureTablesIfNotExists<ApplicationDbContext>(); //can remove after first run;
            }
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
            // org entity store
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
            // data stores
            services.AddSingleton<IDataStore<Amphora.Common.Models.Amphora, byte[]>, InMemoryDataStore<Amphora.Common.Models.Amphora, byte[]>>();
            services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, InMemoryDataStore<Amphora.Common.Models.Tempora, JObject>>();

            //temporae
            services.AddSingleton<IOrgEntityStore<Amphora.Common.Models.Tempora>, InMemoryOrgEntityStore<Amphora.Common.Models.Tempora>>();

            // schemas
            services.AddSingleton<IEntityStore<Schema>, InMemoryEntityStore<Schema>>();

            // orgs
            services.AddSingleton<IEntityStore<Organisation>, InMemoryEntityStore<Organisation>>();
            // this isnt actually in memory :()
            // services.AddSingleton<IDataStore<Amphora.Common.Models.Tempora, JObject>, TemporaEventHubDataStore>();
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
