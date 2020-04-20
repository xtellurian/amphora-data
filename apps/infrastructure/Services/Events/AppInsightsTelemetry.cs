using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.ApplicationInsights;

namespace Amphora.Infrastructure.Services.Events
{
    public class AppInsightsTelemetry : ITelemetry
    {
        private readonly TelemetryClient client;

        public AppInsightsTelemetry(TelemetryClient client)
        {
            this.client = client;
        }

        public Task PublishEventAsync(params IEvent[] events)
        {
            foreach (var e in events)
            {
                client.TrackEvent(e.EventType, e.Data);
            }

            return Task.CompletedTask;
        }

        public void TrackMetricValue(string metric, double? value)
        {
            client.GetMetric(metric).TrackValue(value);
        }
    }
}