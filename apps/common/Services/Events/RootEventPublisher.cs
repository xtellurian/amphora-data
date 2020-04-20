using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Services.Events
{
    public class RootEventPublisher : IEventRoot
    {
        private readonly ITelemetry telemetryClient;
        private readonly IEventPublisher eventPublisher;

        public RootEventPublisher(ITelemetry telemetryClient, IEventPublisher eventPublisher)
        {
            this.telemetryClient = telemetryClient;
            this.eventPublisher = eventPublisher;
        }

        public async Task PublishEventAsync(params IEvent[] events)
        {
            var tasks = new List<Task>();
            tasks.Add(eventPublisher.PublishEventAsync(events));
            tasks.Add(telemetryClient.PublishEventAsync(events));
            await Task.WhenAll(tasks);
        }
    }
}