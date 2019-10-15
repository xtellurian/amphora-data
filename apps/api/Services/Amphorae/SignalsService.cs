using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Signals;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Amphora.Api.Services.Amphorae
{
    public class SignalsService : ISignalService
    {
        private readonly EventHubClient eventHubClient;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly ITsiService tsiService;
        private readonly ILogger<SignalsService> logger;

        public SignalsService(IOptionsMonitor<Options.EventHubOptions> options,
                              IUserService userService,
                              IPermissionService permissionService,
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

            this.userService = userService;
            this.permissionService = permissionService;
            this.tsiService = tsiService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<Dictionary<string, object>>> WriteSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, Dictionary<string, object> values)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                var authorized = await permissionService.IsAuthorizedAsync(user, entity, AccessLevels.WriteContents);
                if (authorized)
                {
                    values["amphora"] = entity.Id;
                    if (!values.ContainsKey("t")) values.Add("t", DateTime.UtcNow);
                    await SendToEventHubAsync(values);
                    return new EntityOperationResult<Dictionary<string, object>>(values);
                }
                else
                {
                    return new EntityOperationResult<Dictionary<string, object>>("Write Contents permission is required") { WasForbidden = true };
                }
            }
        }

        public async Task<IEnumerable<QueryResultPage>> GetTsiSignalsAsync(ClaimsPrincipal principal, AmphoraModel entity)
        {
            var user = await userService.ReadUserModelAsync(principal);

            logger.LogInformation($"Getting TSI signals for Amphora Id {entity.Id}");
            var res = new List<QueryResultPage>();
            if (entity.Signals.Count == 0) return res;

            foreach (var s in entity.Signals.Where(s => s.Signal.IsNumeric))
            {
                var r = await this.GetTsiSignalAsync(principal, entity, s.Signal, false);
                res.Add(r);
            }
            return res;


        }
        public async Task<QueryResultPage> GetTsiSignalAsync(ClaimsPrincipal principal, AmphoraModel entity, SignalModel signal, bool includeOtherSignals = true)
        {
            var user = await userService.ReadUserModelAsync(principal);
            using (logger.BeginScope(new LoggerScope<SignalsService>(user)))
            {
                logger.LogInformation($"Getting Signal KeyName {signal.KeyName} for Amphora Id {entity.Id}");
                var variables = new Dictionary<string, Variable>();
                if (entity.Signals.Count == 0) return default(QueryResultPage);
                // add the inital key
                variables.Add(signal.KeyName, new NumericVariable(
                                        value: new Tsx($"$event.{signal.KeyName}"),
                                        aggregation: new Tsx("avg($value)")));

                foreach (var s in entity.Signals.Where(s => s.SignalId != signal.Id))
                {
                    if (s.Signal.IsNumeric) // only numeric signals can be plotted here
                    {
                        variables.Add(s.Signal.KeyName, new NumericVariable(
                                        value: new Tsx($"$event.{s.Signal.KeyName}"),
                                        aggregation: new Tsx("avg($value)"))); // aggregation actually gets ignored
                    }
                }
                IList<string> projections = null;
                if (!includeOtherSignals)
                {
                    projections = new List<string> { signal.KeyName };
                }
                var start = DateTime.UtcNow.AddDays(-30);
                var end = DateTime.UtcNow.AddDays(7);
                var res = await tsiService.RunGetSeriesAsync(new List<object> { entity.Id }, variables, new DateTimeRange(start, end), projections);
                logger.LogInformation($"Got {res.Properties?.Count} properties from  Amphora Id {entity.Id}");
                return res;
            }
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
    }
}
