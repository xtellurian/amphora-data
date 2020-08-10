using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Platform;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Options;
using Amphora.Common.Security;
using Amphora.Common.Services.Users;
using Amphora.Infrastructure.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

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
            services.Configure<TokenManagementOptions>(configuration.GetSection("tokenManagement"));
            services.Configure<ExternalServices>(configuration.GetSection("ExternalServices"));
            var externalServices = new ExternalServices();
            configuration.GetSection("ExternalServices").Bind(externalServices);
            var token = configuration.GetSection("tokenManagement").Get<TokenManagementOptions>();
            var mvcClientSecret = configuration["MvcClientSecret"];

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CommonAuthorizeAttribute.CookieSchemeName;
                options.DefaultAuthenticateScheme = CommonAuthorizeAttribute.CookieSchemeName;
                options.DefaultForbidScheme = CommonAuthorizeAttribute.CookieSchemeName;
                options.DefaultSignInScheme = CommonAuthorizeAttribute.CookieSchemeName;
                options.DefaultSignOutScheme = CommonAuthorizeAttribute.CookieSchemeName;
                // challenge OIDC
                options.DefaultChallengeScheme = CommonAuthorizeAttribute.OidcSchemeName;
            })
                .AddCookie(CommonAuthorizeAttribute.CookieSchemeName, options =>
                {
                    // Configure the client application to use sliding sessions
                    options.SlidingExpiration = true;
                    // Expire the session of 1 hour of inactivity
                    options.ExpireTimeSpan = System.TimeSpan.FromHours(1);
                })
                .AddOpenIdConnect(CommonAuthorizeAttribute.OidcSchemeName, options =>
                {
                    options.SignInScheme = CommonAuthorizeAttribute.CookieSchemeName;
                    options.CorrelationCookie.Path = "/";
                    options.NonceCookie.Path = "/";

                    options.Authority = externalServices.IdentityUri().ToString();
                    options.RequireHttpsMetadata = false;
                    options.ClientSecret = mvcClientSecret;
                    options.ClientId = OAuthClients.WebApp;
                    options.SaveTokens = true;
                    options.Scope.Add("email");
                    options.Scope.Add(Scopes.AmphoraScope);

                    options.Events.OnTicketReceived = (context) =>
                    {
                        // expire in 24 hours
                        context.Properties.ExpiresUtc = System.DateTime.UtcNow.AddHours(24);
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
                {
                    x.RequireHttpsMetadata = hostingEnvironment.IsProduction();
                    x.Authority = externalServices.IdentityUri().ToString();
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
                });

            services.AddScoped<IUserDataService, ApplicationUserDataService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(GlobalAdminRequirement.GlobalAdminPolicyName, policy =>
                    policy.Requirements.Add(new GlobalAdminRequirement()));
                // configure that the require purchase policy needs the purchase claim
                options.AddPolicy(Policies.RequirePurchaseClaim, p => p.RequireClaim(Claims.Purchase));
            });

            services.AddScoped<IAuthenticateService, IdentityServerService>();
            services.AddScoped<IIdentityService, IdentityServerService>();

            services.AddScoped<IAuthorizationHandler, AmphoraAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, GlobalAdminAuthorizationHandler>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IInvitationService, InvitationService>();

            if (hostingEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
        }
    }
}
