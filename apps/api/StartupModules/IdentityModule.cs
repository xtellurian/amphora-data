using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
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
            services.Configure<RegistrationOptions>(Configuration.GetSection("Registration"));
            services.AddTransient<IUserService, UserService>();

            var key = Configuration.GetSection("Cosmos")["Key"];
            var endpoint = Configuration.GetSection("Cosmos")["Endpoint"];
            var database = Configuration.GetSection("Cosmos")["Database"];
            if (key != null && endpoint != null && database != null)
            {
                services.AddScoped<ISignInManager, SignInManagerWrapper<ApplicationUser>>();
                services.AddTransient<IUserManager, UserManagerWrapper<ApplicationUser>>();

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
            else
            {
                services.AddScoped<ISignInManager, SignInManagerWrapper<TestApplicationUser>>();
                services.AddTransient<IUserManager, UserManagerWrapper<TestApplicationUser>>();


                services.AddDefaultIdentity<TestApplicationUser>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<TestUserContext>();

                services.AddDbContext<TestUserContext>(options => options.UseInMemoryDatabase("test_users"));
            }


        }
        public class TestUserContext : IdentityDbContext<TestApplicationUser>
        {
            public TestUserContext()
            { }

            public TestUserContext(DbContextOptions<TestUserContext> options)
                : base(options)
            { }

        }
    }
}
