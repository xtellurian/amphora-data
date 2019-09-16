using System.Text;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Amphora.Api.Services.Amphorae
{
    public class SignalsService: ISignalService
    {
        private readonly EventHubClient eventHubClient;

        public SignalsService(IOptionsMonitor<Options.EventHubOptions> options)
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
        public async Task WriteSignalAsync(AmphoraModel entity, Datum d)
        {
            d.Amphora = entity.AmphoraId;
             var content = JsonConvert.SerializeObject(d,
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