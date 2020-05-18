using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class ClientTrackerMiddleware
    {
        private const string VersionHeader = "amphora-client-version";
        private ILogger logger;
        private RequestDelegate _next;

        public ClientTrackerMiddleware(RequestDelegate next, ILogger<ClientTrackerMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, ITelemetry telemetry)
        {
            var versionString = "";
            try
            {
                if (httpContext.Request.Path.StartsWithSegments("/api"))
                {
                    if (httpContext.Request.Headers.ContainsKey(VersionHeader))
                    {
                        versionString = httpContext.Request.Headers[VersionHeader].ToArray().FirstOrDefault();
                        var values = versionString?.Split('.')?.ToList();
                        // version string should be in the format 0.10.1
                        if (versionString != null || values?.Count >= 3)
                        {
                            if (IsDigitsOnly(values[0]))
                            {
                                telemetry.TrackMetricValue("version", "major", int.Parse(values[0]));
                            }
                            else
                            {
                                logger.LogWarning($"Cant parse major for {versionString}");
                            }

                            if (IsDigitsOnly(values[1]))
                            {
                                telemetry.TrackMetricValue("version", "minor", int.Parse(values[1]));
                            }
                            else
                            {
                                logger.LogWarning($"Cant parse minor for {versionString}");
                            }

                            if (IsDigitsOnly(values[2]))
                            {
                                telemetry.TrackMetricValue("version", "patch", int.Parse(values[2]));
                            }
                            else
                            {
                                logger.LogWarning($"Cant parse patch for {versionString}");
                            }
                        }
                        else
                        {
                            logger.LogWarning($"Version header was bad: {versionString}");
                        }
                    }
                    else
                    {
                        logger.LogWarning("Version header was not found");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.LogWarning($"Failed to log version, version was: {versionString}", ex);
            }

            await _next(httpContext);
        }

        private bool IsDigitsOnly(string str)
        {
            if (str == null)
            {
                return false;
            }

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}