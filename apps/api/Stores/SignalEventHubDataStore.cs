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
    public class SignalEventHubDataStore : IDataStore<Amphora.Common.Models.AmphoraModel, Datum>
    {
        private readonly EventHubClient eventHubClient;

        public SignalEventHubDataStore(IOptionsMonitor<Options.EventHubOptions> options)
        {
            if (options.CurrentValue.ConnectionString != null)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(options.CurrentValue.ConnectionString)
                {
                    EntityPath = options.CurrentValue.Name
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }

        }

        public Task<Datum> GetDataAsync(Common.Models.AmphoraModel entity, string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> ListNamesAsync(Common.Models.AmphoraModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Datum> SetDataAsync(Common.Models.AmphoraModel entity, Datum data, string name)
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