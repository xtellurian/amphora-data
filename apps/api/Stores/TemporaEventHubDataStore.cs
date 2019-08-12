using System;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Stores
{
    public class TemporaEventHubDataStore : IDataStore<Amphora.Common.Models.Tempora, Datum>
    {
        private readonly EventHubClient eventHubClient;

        public TemporaEventHubDataStore(IOptionsMonitor<Options.EventHubOptions> options)
        {
            if (options.CurrentValue.EventHubConnectionString != null)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(options.CurrentValue.EventHubConnectionString)
                {
                    EntityPath = options.CurrentValue.EventHubName
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }

        }
        public Datum GetData(Tempora entity)
        {
            throw new System.NotImplementedException();
        }

        public Datum SetData(Tempora entity, Datum data)
        {
            var content = JsonConvert.SerializeObject(data,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
            eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content))).Wait();
            return data;
        }
        public async Task<string> SetDataAsync(Tempora entity, string data)
        {
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(data)));
            return data;
        }
    }
}