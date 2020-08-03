using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Amphora.Infrastructure.Database.Cache.RateLimiter
{
    public class CustomRateLimitConfiguration : RateLimitConfiguration
    {
        public CustomRateLimitConfiguration(IHttpContextAccessor httpContextAccessor,
                                            IOptions<IpRateLimitOptions> ipOptions,
                                            IOptions<ClientRateLimitOptions> clientOptions)
        : base(httpContextAccessor, ipOptions, clientOptions)
        {
        }

        protected override void RegisterResolvers()
        {
            base.RegisterResolvers();

            IpResolvers.Add(new CustomIpResolver(HttpContextAccessor, IpRateLimitOptions.RealIpHeader));
        }
    }
}