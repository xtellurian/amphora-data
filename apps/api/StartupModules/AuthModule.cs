using System.Text;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Amphora.Api.StartupModules
{
    public class AuthModule
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;

        public AuthModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.hostingEnvironment = env;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenManagementOptions>(configuration.GetSection("tokenManagement"));
            var token = configuration.GetSection("tokenManagement").Get<TokenManagementOptions>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddAuthentication().AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = hostingEnvironment.IsProduction();
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy(GlobalAdminRequirement.GlobalAdminPolicyName, policy =>
                    policy.Requirements.Add(new GlobalAdminRequirement()));
            });

            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IAuthorizationHandler, AmphoraAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, GlobalAdminAuthorizationHandler>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IInvitationService, InvitationService>();
        }
    }
}
