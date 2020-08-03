using Amphora.Infrastructure.Database.Cache.RateLimiter;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Infrastructure.Database.Cache
{
    public static class RateLimiterProvider
    {
        public static void UseRateLimitingByIpAddress(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            // load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            if (isDevelopment)
            {
                services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            }
            else
            {
                services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            }

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
        }

        public static void UseRateLimitingByClientId(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            // load general configuration from appsettings.json
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));

            // load client rules from appsettings.json
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

            // inject counter and rules stores
            if (isDevelopment)
            {
                services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            }
            else
            {
                services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            }

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
        }

        public static void UseIpRateLimiter(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
        }
    }
}