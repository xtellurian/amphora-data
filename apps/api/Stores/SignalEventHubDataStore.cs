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
    public class SignalEventHubDataStore : IDataStore<Amphora.Common.Models.Amphora, Datum>
    {
        private readonly EventHubClient eventHubClient;

        public SignalEventHubDataStore(IOptionsMonitor<Options.EventHubOptions> options)
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

        public Task<Datum> GetDataAsync(Common.Models.Amphora entity, string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> ListNamesAsync(Common.Models.Amphora entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Datum> SetDataAsync(Common.Models.Amphora entity, Datum data, string name)
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