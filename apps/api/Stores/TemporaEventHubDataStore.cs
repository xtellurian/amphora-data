using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
        public Task<Datum> GetDataAsync(Tempora entity)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Datum> SetDataAsync(Tempora entity, Datum data)
        {
            var content = JsonConvert.SerializeObject(data,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                });
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content)));
            return data;
        }
    }
}