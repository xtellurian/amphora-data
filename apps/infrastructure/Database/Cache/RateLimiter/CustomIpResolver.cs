using System;
using System.Linq;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace Amphora.Infrastructure.Database.Cache.RateLimiter
{
    public class CustomIpResolver : IIpResolveContributor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CustomIpResolver(IHttpContextAccessor httpContextAccessor, string ipHeader)
        {
            this.httpContextAccessor = httpContextAccessor;
            IpHeader = ipHeader;
        }

        public string IpHeader { get; }

        public string ResolveIp()
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new NullReferenceException("Http Context was null");
            }

            var defaultIp = context.Connection.RemoteIpAddress.ToString();

            if (context.Request.Headers.TryGetValue(IpHeader, out var stringValues))
            {
                return stringValues.FirstOrDefault() ?? defaultIp;
            }
            else
            {
                return defaultIp;
            }
        }
    }
}