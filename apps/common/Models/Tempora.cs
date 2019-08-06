using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Tempora : DataEntity, IDataEntity
    {
        public string SchemaId { get; set; }
        public string Title {get;set;}
        public string Description {get;set;}
        public double Price {get;set;}
    }
}