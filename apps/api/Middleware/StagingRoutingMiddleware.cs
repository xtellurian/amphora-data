using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class StagingRoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StagingRoutingMiddleware> logger;

        public StagingRoutingMiddleware(RequestDelegate next, ILogger<StagingRoutingMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        private const string StagingRouteKey = "x-ms-routing-name";
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Query.TryGetValue(StagingRouteKey, out var values))
            {
                var value = values.ToArray().FirstOrDefault();
                if (value != null)
                {
                    logger.LogInformation($"Setting {StagingRouteKey} cookie to {value}");
                    httpContext.Response.Cookies.Append(StagingRouteKey, value);
                }
            }

            await _next(httpContext);
        }
    }
}