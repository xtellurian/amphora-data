namespace Amphora.Common.Models
{
    public abstract class MarketEntity : OrgEntity
    {
        public abstract DataEntityTypes GetEntityType();
        public string Title {get;set;}
        public string Description {get;set;}
        public double Price {get;set;}
    }
}