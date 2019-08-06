using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Tempora : SearchableDataEntity, IOrgEntity
    {
        public string SchemaId { get; set; }

        public override DataEntityTypes GetEntityType()
        {
            return DataEntityTypes.Tempora;
        }
    }
}