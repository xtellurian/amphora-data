using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Signals;
using Amphora.Common.Services.Azure;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class SignalsService : ISignalService
    {
        private readonly IEventHubSender eventHubSender;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly ITsiService tsiService;
        private readonly ILogger<SignalsService> logger;

        public SignalsService(IEventHubSender eventHubSender,
                              IUserService userService,
                              IPermissionService permissionService,
                              ITsiService tsiService,
                              ILogger<SignalsService> logger)
        {
            this.eventHubSender = eventHubSender;
            this.userService = userService;
            this.permissionService = permissionService;
            this.tsiService = tsiService;
            this.logger = logger;
        }

        public async Task<long> MaxWriteTimeAsync(AmphoraModel entity)
        {
            // 2 year window
            var start = DateTime.UtcNow.AddYears(-1);
            var end = DateTime.UtcNow.AddYears(1);
            var variables = new Dictionary<string, Variable>
            {
                { "maxWt", new AggregateVariable(new Tsx("max($event.wt)")) }
            };

            var result = await tsiService.RunGetAggregateAsync(new List<object> { entity.Id },
                variables,
                new DateTimeRange(start, end),
                TimeSpan.FromDays(365),
                new List<string> { "maxWt" });

            var max = result.Properties.FirstOrDefault(_ => _.Name == "maxWt").Values.Max().ToString();
            if (long.TryParse(max, System.Globalization.NumberStyles.Float, null, out var m))
            {
                return m;
            }

            return DateTime.MaxValue.Ticks;
        }

        public async Task<EntityOperationResult<Dictionary<string, object>>> WriteSignalAsync(ClaimsPrincipal principal,
                                                                                              AmphoraModel entity,
                                                                                              Dictionary<string, object> values)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                var authorized = await permissionService.IsAuthorizedAsync(user, entity, AccessLevels.WriteContents);
                if (authorized)
                {
                    if (InvalidSignalProperties(entity, values))
                    {
                        return new EntityOperationResult<Dictionary<string, object>>(user, "Invalid properties");
                    }

                    AddDefaultSignalProperties(entity, values);
                    await eventHubSender.SendToEventHubAsync(values);
                    return new EntityOperationResult<Dictionary<string, object>>(user, values);
                }
                else
                {
                    return new EntityOperationResult<Dictionary<string, object>>(user, "Write Contents permission is required") { WasForbidden = true };
                }
            }
        }

        private bool InvalidSignalProperties(AmphoraModel entity, Dictionary<string, object> values)
        {
            var isInvalid = false;
            foreach (var s in values.Keys)
            {
                // check if key is not in the signals, and its not a timestamp
                if (!entity.V2Signals.Any(_ => _.Property == s) && s != SpecialProperties.Timestamp) { isInvalid = true; }
                // check that numeric values are numeric
                var property = entity.V2Signals.FirstOrDefault(_ => _.Property == s);
                if (property != null) // just check incase
                {
                    var value = values[property.Property];
                    if (value is string && property.IsNumeric)
                    {
                        isInvalid = true;
                    }
                }
            }

            return isInvalid;
        }

        private static void AddDefaultSignalProperties(AmphoraModel entity, Dictionary<string, object> values)
        {
            values[SpecialProperties.TimeSeriesId] = entity.Id;
            if (!values.ContainsKey(SpecialProperties.Timestamp)) { values.Add(SpecialProperties.Timestamp, DateTime.UtcNow); }
            values[SpecialProperties.WriteTime] = DateTime.UtcNow.Ticks;
        }

        public async Task<EntityOperationResult<IEnumerable<Dictionary<string, object>>>> WriteSignalBatchAsync(ClaimsPrincipal principal, AmphoraModel entity, IEnumerable<Dictionary<string, object>> values)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                var authorized = await permissionService.IsAuthorizedAsync(user, entity, AccessLevels.WriteContents);
                if (authorized)
                {
                    foreach (var value in values)
                    {
                        if (InvalidSignalProperties(entity, value))
                        {
                            return new EntityOperationResult<IEnumerable<Dictionary<string, object>>>(user, "Invalid properties");
                        }

                        AddDefaultSignalProperties(entity, value);
                    }

                    await eventHubSender.SendToEventHubAsync(values);
                    return new EntityOperationResult<IEnumerable<Dictionary<string, object>>>(user, values);
                }
                else
                {
                    return new EntityOperationResult<IEnumerable<Dictionary<string, object>>>(user, "Write Contents permission is required") { WasForbidden = true };
                }
            }
        }

        public async Task<QueryResultPage> GetTsiSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, SignalV2 signal, bool includeOtherSignals = false)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                logger.LogInformation($"Getting Signal Property {signal.Property} for Amphora Id {entity.Id}");
                var variables = new Dictionary<string, Variable>();
                if (entity.V2Signals.Count == 0) { return default(QueryResultPage); }
                // add the inital key
                variables.Add(signal.Property, new NumericVariable(
                                        value: new Tsx($"$event.{signal.Property}"),
                                        aggregation: new Tsx("avg($value)")));

                foreach (var s in entity.V2Signals.Where(s => s.Id != signal.Id))
                {
                    if (s.IsNumeric) // only numeric signals can be plotted here
                    {
                        variables.Add(s.Property, new NumericVariable(
                                        value: new Tsx($"$event.{s.Property}"),
                                        aggregation: new Tsx("avg($value)"))); // aggregation actually gets ignored
                    }
                }

                IList<string> projections = null;
                if (!includeOtherSignals)
                {
                    projections = new List<string> { signal.Property };
                }

                var start = DateTime.UtcNow.AddDays(-30);
                var end = DateTime.UtcNow.AddDays(7);
                var res = await tsiService.RunGetSeriesAsync(new List<object> { entity.Id }, variables, new DateTimeRange(start, end), projections);
                logger.LogInformation($"Got {res.Properties?.Count} properties from  Amphora Id {entity.Id}");
                return res;
            }
        }

        public async Task<IDictionary<SignalV2, IEnumerable<string>>> GetUniqueValuesForStringProperties(ClaimsPrincipal principal, AmphoraModel entity)
        {
            var user = await userService.ReadUserModelAsync(principal);

            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                var start = DateTime.UtcNow.AddDays(-30);
                var end = DateTime.UtcNow.AddDays(7);
                var stringSignals = entity.V2Signals.Where(s => s.IsString);
                var result = await tsiService.RunGetEventsAsync(new List<object> { entity.Id },
                    new DateTimeRange(start, end),
                    stringSignals.Select(s => s.Property).ToList());
                var dic = new Dictionary<SignalV2, IEnumerable<string>>();

                foreach (var s in stringSignals)
                {
                    var p = result?.Properties?.FirstOrDefault(_ => _?.Name == s.Property);
                    if (p?.Values != null) { dic.Add(s, p.Values.Where(_ => _ != null).Select(_ => _.ToString()).Distinct()); }
                }

                return dic;
            }
        }
    }
}
