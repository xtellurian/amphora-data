using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Users;
using Amphora.Api.Options;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Wrappers;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Amphora.Api.StartupModules
{
    public class IdentityModule
    {
        private readonly IWebHostEnvironment HostingEnvironment;
        private readonly IConfiguration Configuration;

        public IdentityModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RegistrationOptions>(Configuration.GetSection("Registration"));
            services.AddTransient<IUserService, UserService>();

            var key = Configuration.GetSection("Cosmos")["PrimaryKey"];
            var endpoint = Configuration.GetSection("Cosmos")["Endpoint"];
            var database = Configuration.GetSection("Cosmos")["Database"];
            services.AddScoped<ISignInManager, SignInManagerWrapper<ApplicationUser>>();
            services.AddTransient<IUserManager, UserManagerWrapper<ApplicationUser>>();


            services.AddDefaultIdentity<ApplicationUser>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<AmphoraContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = Boolean.Parse(Configuration
                    .GetSection("IdentityConfiguration")
                    .GetSection("SignIn")["RequireConfirmedEmail"] ?? "false");
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
        }
    }
}
