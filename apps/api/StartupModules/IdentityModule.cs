using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class IdentityModule
    {
        private readonly IHostingEnvironment HostingEnvironment;
        private readonly IConfiguration Configuration;

        public IdentityModule(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISignInManager<ApplicationUser>, SignInManagerWrapper<ApplicationUser>>();
            services.AddScoped<IUserManager<ApplicationUser>, UserManagerWrapper<ApplicationUser>>();

            var key = Configuration.GetSection("Cosmos")["Key"];
            var endpoint = Configuration.GetSection("Cosmos")["Endpoint"];
            var database = Configuration.GetSection("Cosmos")["Database"];

            if(key != null && endpoint != null && database != null)
            {
                services.AddIdentityWithDocumentDBStores<ApplicationUser, Microsoft.AspNetCore.Identity.DocumentDB.IdentityRole>(
                dbOptions =>
                {
                    dbOptions.DocumentUrl = endpoint;
                    dbOptions.DocumentKey = key;
                    dbOptions.DatabaseId = database;
                    dbOptions.CollectionId = "Identity";

                    // optional:
                    // dbOptions.PartitionKey = [provide definition here];
                },
                identityOptions =>
                {
                    identityOptions.User.RequireUniqueEmail = true;
                })
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4);
            }
        }
    }
}
