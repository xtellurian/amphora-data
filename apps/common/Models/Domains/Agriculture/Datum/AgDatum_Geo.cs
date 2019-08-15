using Amphora.Common.Attributes;

namespace Amphora.Common.Models.Domains.Agriculture
{
    public partial class AgDatum : Datum
    {
        [DatumMember("latitude", "float?", null, "Latitude")]
        public float? Latitude { get; set; }
        [DatumMember("longitude", "float?", null, "Longitude")]
        public float? Longitude { get; set; }
    }
}