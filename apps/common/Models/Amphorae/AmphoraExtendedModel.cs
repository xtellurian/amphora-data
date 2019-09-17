using Amphora.Common.Models.Domains;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraExtendedModel : AmphoraModel
    {
        public string Description { get; set; }
        public string GeoHash { get; set; }
       
    }
}