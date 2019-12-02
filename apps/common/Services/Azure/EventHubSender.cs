using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Extensions;
using Amphora.Common.Options;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Amphora.Common.Services.Azure
{
    public class EventHubSender: IDisposable
    {
        private const int MAX_EVENTS = 500;
        private readonly ILogger<EventHubSender> logger;
        private readonly EventHubClient? eventHubClient;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public EventHubSender(IOptionsMonitor<EventHubOptions> options, ILogger<EventHubSender> logger)
        {
            this.logger = logger;

            if (options.CurrentValue.ConnectionString != null)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(options.CurrentValue.ConnectionString)
                {
                    EntityPath = options.CurrentValue.Name
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }
        }

        public void Dispose()
        {
            this.logger.LogInformation("Closing Event Hub Client");
            this.eventHubClient?.Close();
        }

        public async Task SendToEventHubAsync(IEnumerable<Dictionary<string, object?>> signals)
        {
            foreach(var s in signals.Batch(MAX_EVENTS))
            {
                var content = JsonConvert.SerializeObject(s,
                                            Newtonsoft.Json.Formatting.None,
                                            jsonSerializerSettings
                                            );
                await SendContentAsync(content);
            }
        }

        public async Task SendToEventHubAsync(Dictionary<string, object> signal)
        {
            var content = JsonConvert.SerializeObject(signal,
                                           Newtonsoft.Json.Formatting.None,
                                           jsonSerializerSettings);
            await SendContentAsync(content);
        }

        private async Task SendContentAsync(string content)
        {
            if (eventHubClient == null)
            {
                logger.LogWarning("EventHubClient is null. Not Sending anything...");
            }
            else
            {
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content)));
            }
        }
    }
}