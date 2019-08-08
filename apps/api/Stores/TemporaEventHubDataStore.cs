using System;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Stores
{
    public class TemporaEventHubDataStore : IDataStore<Amphora.Common.Models.Tempora, JObject>
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
        public JObject GetData(Tempora entity)
        {
            throw new System.NotImplementedException();
        }

        public JObject SetData(Tempora entity, JObject data)
        {
            data = AddPropertiesIfRequired(entity, data);
            var content = JsonConvert.SerializeObject(data);
            eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content))).Wait();
            return data;
        }
        public async Task<string> SetDataAsync(Tempora entity, string data)
        {
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(data)));
            return data;
        }

        const string timeKey = "t";
        const string temporaKey = "tempora";
        private JObject AddPropertiesIfRequired(Tempora entity, JObject data)
        {
            if (!data.ContainsKey(timeKey)) data[timeKey] = DateTime.Now;
            data[temporaKey] = entity.Id;
            return data;
        }
    }
}