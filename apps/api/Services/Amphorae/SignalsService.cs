using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Signals;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.TimeSeriesInsights.Models;
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
        private readonly ITsiService tsiService;
        private readonly ILogger<SignalsService> logger;

        public SignalsService(IOptionsMonitor<Options.EventHubOptions> options,
                              ITsiService tsiService,
                              ILogger<SignalsService> logger)
        {
            if (options.CurrentValue.ConnectionString != null)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(options.CurrentValue.ConnectionString)
                {
                    EntityPath = options.CurrentValue.Name
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }

            this.tsiService = tsiService;
            this.logger = logger;
        }
        public async Task WriteSignalAsync(AmphoraModel entity, JObject jObj)
        {
            var signal = new Dictionary<string, object>();
            signal.Add("amphora", entity.Id);
            foreach (var s in entity.Signals)
            {
                if (s.Signal.ValueType == SignalModel.Numeric)
                {
                    var v = jObj.GetValue(s.Signal.KeyName)?.Value<double?>();
                    if (v.HasValue) signal.Add(s.Signal.KeyName, v);
                }
                else if (s.Signal.ValueType == SignalModel.String)
                {
                    var v = jObj.GetValue(s.Signal.KeyName)?.Value<string>();
                    if (!string.IsNullOrEmpty(v)) signal.Add(s.Signal.KeyName, v);
                }
            }
            // at timestamp
            if (jObj.TryGetValue("t", out var token))
            {
                try
                {
                    signal["t"] = token.Value<DateTime>();
                }
                catch (System.FormatException ex)
                {
                    logger.LogError($"Amphora {entity.Id} signal had bad timestamp. Defaulting to now", ex);
                    signal["t"] = DateTime.UtcNow;
                }
            }
            else
            {
                signal["t"] = DateTime.UtcNow;
            }
            await SendToEventHubAsync(signal);
        }

        public async Task WriteSignalAsync(AmphoraModel entity, Dictionary<string, object> values)
        {
            values["amphora"] = entity.Id;
            values.Add("t", DateTime.UtcNow);
            await SendToEventHubAsync(values);
        }

        private async Task SendToEventHubAsync(Dictionary<string, object> signal)
        {
            var content = JsonConvert.SerializeObject(signal,
                                           Newtonsoft.Json.Formatting.None,
                                           new JsonSerializerSettings
                                           {
                                               NullValueHandling = NullValueHandling.Ignore,
                                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                                           });
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(content)));
        }

        public async Task<IEnumerable<QueryResultPage>> GetTsiSignalsAsync(AmphoraModel entity)
        {
            var variables = new Dictionary<string, Variable>();
            var res = new List<QueryResultPage>();
            if (entity.Signals.Count == 0) return res;

            foreach (var s in entity.Signals)
            {
                variables.Add(s.Signal.KeyName, new NumericVariable(
                                    value: new Tsx($"$event.{s.Signal.KeyName}"),
                                    aggregation: new Tsx("avg($value)")));

                var r = await this.GetTsiSignalAsync(entity, s.Signal, false);
                res.Add(r);
            }
            return res;
        }
        public async Task<QueryResultPage> GetTsiSignalAsync(AmphoraModel entity, SignalModel signal, bool includeOtherSignals = true)
        {
            var variables = new Dictionary<string, Variable>();
            if (entity.Signals.Count == 0) return default(QueryResultPage);
            // add the inital key
            variables.Add(signal.KeyName, new NumericVariable(
                                    value: new Tsx($"$event.{signal.KeyName}"),
                                    aggregation: new Tsx("avg($value)")));

            foreach (var s in entity.Signals.Where(s => s.SignalId != signal.Id))
            {
                variables.Add(s.Signal.KeyName, new NumericVariable(
                                    value: new Tsx($"$event.{s.Signal.KeyName}"),
                                    aggregation: new Tsx("avg($value)")));
            }
            IList<string> projections = null;
            if (!includeOtherSignals)
            {
                projections = new List<string> { signal.KeyName };
            }
            var start = DateTime.UtcNow.AddDays(-30);
            var end = DateTime.UtcNow;
            var res = await tsiService.RunGetSeriesAsync(new List<object> { entity.Id }, variables, new DateTimeRange(start, end), projections);
            return res;
        }


    }
}