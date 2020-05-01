using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Microsoft.Extensions.Logging;

namespace Amphora.Common.Services.Events
{
    public class RootEventPublisher : IEventRoot
    {
        private readonly ITelemetry telemetryClient;
        private readonly IEventPublisher eventPublisher;
        private readonly ILogger<RootEventPublisher> logger;

        public RootEventPublisher(ITelemetry telemetryClient,
                                  IEventPublisher eventPublisher,
                                  ILogger<RootEventPublisher> logger)
        {
            this.telemetryClient = telemetryClient;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task PublishEventAsync(params IEvent[] events)
        {
            var tasks = new List<Task>();
            try
            {
                var publisherTask = eventPublisher.PublishEventAsync(events);
                var telemetryTask = telemetryClient.PublishEventAsync(events);
                tasks.Add(publisherTask);
                tasks.Add(telemetryTask);

                // wait for all tasks to complete
                await Task.WhenAll(tasks);

                if (!publisherTask.IsCompletedSuccessfully)
                {
                    logger.LogError("publisherTask did not complete successfully", publisherTask.Exception);
                }

                if (!telemetryTask.IsCompletedSuccessfully)
                {
                    logger.LogError("TelemetryTask did not complete successfully", telemetryTask.Exception);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Publisher threw exception", ex);
            }
        }
    }
}