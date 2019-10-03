using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Amphora.Api.Services.Amphorae
{
    public class SignalsService : ISignalService
    {
        private readonly EventHubClient eventHubClient;
        private readonly ILogger<SignalsService> logger;

        public SignalsService(IOptionsMonitor<Options.EventHubOptions> options, ILogger<SignalsService> logger)
        {
            if (options.CurrentValue.ConnectionString != null)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(options.CurrentValue.ConnectionString)
                {
                    EntityPath = options.CurrentValue.Name
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }

            this.logger = logger;
        }
        public async Task WriteSignalAsync(AmphoraModel entity, JObject jObj)
        {
            var signal = new Dictionary<string, object>();
            signal.Add("amphora", entity.Id);
            foreach(var s in entity.Signals)
            {
                if(s.Signal.ValueType == SignalModel.Numeric)
                {
                    var v = jObj.GetValue(s.Signal.KeyName)?.Value<double?>();
                    if(v.HasValue) signal.Add(s.Signal.KeyName, v);
                }
                else if(s.Signal.ValueType == SignalModel.String)
                {
                    var v = jObj.GetValue(s.Signal.KeyName)?.Value<string>();
                    if(!string.IsNullOrEmpty(v)) signal.Add(s.Signal.KeyName, v);
                }
            }
            // at timestamp
            if(jObj.TryGetValue("t", out var token))
            {
                try
                {
                    signal["t"] = token.Value<DateTime>();
                }
                catch(System.FormatException ex)
                {
                    logger.LogError($"Amphora {entity.Id} signal had bad timestamp. Defaulting to now", ex);
                    signal["t"] = DateTime.UtcNow;
                }
            }
            else
            {
                signal["t"] = DateTime.UtcNow;
            }
            var content = JsonConvert.SerializeObject(signal,
                               Newtonsoft.Json.Formatting.None,
                               new JsonSerializerSettings
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   ContractResolver = new CamelCasePropertyNamesContractResolver()
                               });
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content)));
        }
    }
}