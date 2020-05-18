using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace Amphora.Infrastructure.Services.Events
{
    public class AppInsightsTelemetry : ITelemetry
    {
        private readonly TelemetryClient client;
        private readonly ILogger<AppInsightsTelemetry> logger;

        public AppInsightsTelemetry(TelemetryClient client, ILogger<AppInsightsTelemetry> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public Task PublishEventAsync(params IEvent[] events)
        {
            foreach (var e in events)
            {
                client.TrackEvent(e.EventType, e.Data);
            }

            logger.LogInformation($"Tracked {events.Count()} Events");
            return Task.CompletedTask;
        }

        public void TrackMetricValue(string metric, double? value)
        {
            client.GetMetric(metric).TrackValue(value);
            logger.LogInformation($"Tracked metric | {metric} = {value}");
        }
    }
}