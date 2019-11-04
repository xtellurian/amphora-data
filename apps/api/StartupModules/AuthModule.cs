using System.Text;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Amphora.Api.Services.Platform;

namespace Amphora.Api.StartupModules
{
    public class AuthModule
    {
        private readonly IWebHostEnvironment HostingEnvironment;
        private readonly IConfiguration Configuration;

        public AuthModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenManagementOptions>(Configuration.GetSection("tokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagementOptions>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddAuthentication()
            .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = HostingEnvironment.IsProduction();
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                   ValidIssuer = token.Issuer,
                   ValidAudience = token.Audience,
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           }).AddCookie();
           
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IAuthorizationHandler, AmphoraAuthorizationHandler>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IInvitationService, InvitationService>();
        }
    }
}
