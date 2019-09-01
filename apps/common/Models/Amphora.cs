using Amphora.Common.Contracts;
using Amphora.Common.Models.Domains;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models
{
    public class Amphora : Entity
    {
        public Amphora()
        {
            // set some defaults
            OrgId = "default";
        }
        public string AmphoraId { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DomainId DomainId { get; set; }
        public string OrgId { get; set; }
        public string GeoHash { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
    }
}