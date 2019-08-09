using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Tempora : MarketEntity, IOrgEntity
    {
        public Tempora()
        {
            this.OrgId = "default";
        }
        public string SchemaId { get; set; }

        public override DataEntityTypes GetEntityType()
        {
            return DataEntityTypes.Tempora;
        }
    }
}