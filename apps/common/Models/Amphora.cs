using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Amphora: DataEntity, IDataEntity
    {
        public string SchemaId { get; set; }
        public bool Bounded { get; set; } = true; // a bounded amphora cannot be appended to
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}