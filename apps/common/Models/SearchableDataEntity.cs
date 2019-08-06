namespace Amphora.Common.Models
{
    public abstract class SearchableDataEntity : DataEntity
    {
        public abstract DataEntityTypes GetEntityType();
        public string Title {get;set;}
        public string Description {get;set;}
        public double Price {get;set;}
    }
}