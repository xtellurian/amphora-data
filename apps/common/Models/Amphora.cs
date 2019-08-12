using Amphora.Common.Contracts;

namespace Amphora.Common.Models
{
    public class Amphora : MarketEntity, IOrgScoped
    {
        public Amphora()
        {
            // set some defaults
            ContentType = ContentTypes.OctetStream;
            OrgId = "default";
        }
        public string ContentType { get; set; }
        public string FileName { get; set; } = "data";

        public override DataEntityTypes GetEntityType()
        {
            return DataEntityTypes.Amphora;
        }
    }
}