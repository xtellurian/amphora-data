using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Tempora : MarketEntity, IOrgScoped
    {
        public Tempora()
        {
            this.OrgId = "default";
        }
        public string DomainId { get; set; }

        public override DataEntityTypes GetEntityType()
        {
            return DataEntityTypes.Tempora;
        }
    }
}