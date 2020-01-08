using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Api.Options;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Wrappers;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class IdentityModule
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;

        public IdentityModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.hostingEnvironment = env;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RegistrationOptions>(configuration.GetSection("Registration"));
            services.AddTransient<IUserService, UserService>();

            var key = configuration.GetSection("Cosmos")["PrimaryKey"];
            var endpoint = configuration.GetSection("Cosmos")["Endpoint"];
            var database = configuration.GetSection("Cosmos")["Database"];
            services.AddScoped<ISignInManager, SignInManagerWrapper<ApplicationUser>>();
            services.AddTransient<IUserManager, UserManagerWrapper<ApplicationUser>>();

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = bool.Parse(configuration
                    .GetSection("IdentityConfiguration")
                    .GetSection("SignIn")["RequireConfirmedEmail"] ?? "false");
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AmphoraContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
        }
    }
}
