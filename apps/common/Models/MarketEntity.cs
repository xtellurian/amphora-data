using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public abstract class MarketEntity : Entity, IOrgScoped
    {
        public string OrgId { get; set; }
        public abstract DataEntityTypes GetEntityType();
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}