// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Amphora.Api.Services.Events;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Users;
using Amphora.Identity.EntityFramework;
using Amphora.Identity.Services;
using Amphora.Identity.Stores;
using Amphora.Infrastructure.Database.EFCoreProviders;
using Amphora.Infrastructure.Models.Options;
using Amphora.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amphora.Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        private bool IsUsingCosmos() => Environment.IsProduction() || Configuration.IsPersistentStores();
        private bool IsUsingSql() => Environment.IsProduction() || Configuration["sql"] == "true";

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            System.Console.WriteLine($"Hosting Environment Name is {Environment.EnvironmentName}");

            if (IsUsingCosmos())
            {
                var cosmosOptions = new CosmosOptions();
                Configuration.GetSection("Cosmos").Bind(cosmosOptions);
                services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));
                services.UseCosmos<IdentityContext>(cosmosOptions);
            }
            else if (IsUsingSql())
            {
                var sqlOptions = new SqlServerOptions();
                Configuration.GetSection("SqlServer").Bind(sqlOptions);
                services.UseSqlServer<IdentityContext>(sqlOptions);
            }
            else if (Environment.IsDevelopment())
            {
                services.UseInMemory<IdentityContext>();
            }
            else
            {
                throw new System.ApplicationException("No DB Context Configured");
            }

            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Account/Logout";
                })
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddProfileService<IdentityProfileService>()
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddScoped<IUserService, UserService>();
            if (Configuration.IsPersistentStores() || Environment.IsProduction())
            {
                services.Configure<AzureEventGridTopicOptions>("AppTopic", Configuration.GetSection("EventGrid").GetSection("AppTopic"));
                services.AddTransient<IEventPublisher, EventGridService>();
            }
            else
            {
                services.AddTransient<IEventPublisher, LoggingEventPublisher>();
            }

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.AddScoped<IEntityStore<ApplicationUser>, UsersEFStore>();
            services.AddScoped<IIdentityService, IdentityServerService>();

            services.AddTransient<IEmailSender, SendGridEmailSender>();

            services.AddAuthentication();

            services.Configure<Amphora.Common.Models.Host.HostOptions>(Configuration.GetSection("Host"));
            services.Configure<ExternalServices>(Configuration.GetSection("ExternalServices"));

            services.AddMvc(opts =>
            {
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Login";
                options.LogoutPath = $"/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied"; // TODO: create access denided oasge
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

            if (!IsUsingCosmos())
            {
                app.MigrateSql<IdentityContext>();
            }
        }
    }
}