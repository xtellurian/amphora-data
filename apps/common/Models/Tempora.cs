using Amphora.Common.Contracts;
using Amphora.Common.Models.Domains;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models
{
    public class Tempora : MarketEntity, IOrgScoped
    {
        public Tempora()
        {
            this.OrgId = "default";
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public DomainId DomainId { get; set; }

        public override DataEntityTypes GetEntityType()
        {
            return DataEntityTypes.Tempora;
        }
    }
}