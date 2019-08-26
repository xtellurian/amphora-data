using System;
using System.Text;
using Amphora.Api.Contracts;
using Amphora.Api.Data;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Api.Services;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            if (HostingEnvironment.IsProduction())
            {
                Console.WriteLine("Disabling Dev SignIn for production");
                //Models.Development.DevSignInResult.Disabled = true;
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
    }
}
