using System.Threading.Tasks;
using Amphora.Api.Extensions;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class ClientTrackerMiddleware
    {
        private ILogger logger;
        private RequestDelegate _next;

        public ClientTrackerMiddleware(RequestDelegate next, ILogger<ClientTrackerMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, ITelemetry telemetry)
        {
            if (httpContext.Request.Path.StartsWithSegments("/api"))
            {
                if (httpContext.Request.TryReadClientVersion(out var info))
                {
                    telemetry.TrackMetricValue("version", "major", info.Major);
                    telemetry.TrackMetricValue("version", "minor", info.Minor);
                    telemetry.TrackMetricValue("version", "patch", info.Patch);
                }
                else
                {
                    logger.LogInformation($"Couldn't get version");
                }
            }

            await _next(httpContext);
        }
    }
}