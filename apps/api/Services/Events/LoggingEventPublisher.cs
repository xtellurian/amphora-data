using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Events
{
    public class LoggingEventPublisher : IEventPublisher
    {
        private readonly ILogger<LoggingEventPublisher> logger;

        public LoggingEventPublisher(ILogger<LoggingEventPublisher> logger)
        {
            this.logger = logger;
        }

        public Task PublishEventAsync(params IEvent[] events)
        {
            foreach (var e in events)
            {
                logger.LogInformation($"Sending Event Type {e.EventType} with subject {e.Subject}");
            }

            return Task.CompletedTask;
        }
    }
}