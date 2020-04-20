using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Options;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Infrastructure.Services.Events
{
    public class EventGridService : IEventPublisher
    {
        private readonly ILogger<EventGridService> logger;
        private TopicCredentials topicCredentials;
        private string topicHostname;

        public EventGridService(IOptionsMonitor<AzureEventGridTopicOptions> options, ILogger<EventGridService> logger)
        {
            var o = options.Get("AppTopic");
            if (o.Endpoint == null || o.PrimaryKey == null)
            {
                throw new System.ArgumentNullException("Null options");
            }

            this.topicCredentials = new TopicCredentials(o.PrimaryKey);
            this.topicHostname = new Uri(o.Endpoint).Host;
            this.logger = logger;
        }

        public async Task PublishEventAsync(params IEvent[] events)
        {
            try
            {
                using (var client = new EventGridClient(topicCredentials))
                {
                    var x = ToEventGridType(events);
                    client.SerializationSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    client.SerializationSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    await client.PublishEventsAsync(topicHostname, x);
                    logger.LogInformation($"Sent {x.Count} events");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical($"Sending events failed, {ex.Message}");
            }
        }

        private IList<EventGridEvent> ToEventGridType(IEnumerable<IEvent> events)
        {
            var result = new List<EventGridEvent>();
            foreach (var e in events)
            {
                result.Add(new EventGridEvent()
                {
                    Id = e.Id,
                    EventType = e.EventType,
                    Data = e.Data,
                    EventTime = e.EventTime,
                    Subject = e.Subject,
                    DataVersion = e.DataVersion
                });
            }

            return result;
        }
    }
}
