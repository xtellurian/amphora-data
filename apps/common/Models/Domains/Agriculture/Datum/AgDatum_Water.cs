using Amphora.Common.Attributes;

namespace Amphora.Common.Models.Domains.Agriculture
{
    public partial class AgDatum : Datum
    {
        [DatumMember("discharge", "float?", "ML/day")]
        public float? Discharge { get; set; }
        [DatumMember("rainfall", "float?", "mm")]
        public float? Rainfall{ get; set; }
        [DatumMember("storagePercent", "float?", "%")]
        public float? StoragePercent { get; set; }
        [DatumMember("volume", "float?", "GL")]
        public float? Volume { get; set; }
        [DatumMember("waterLevel", "float?", "metres")]
        public float? WaterLevel { get; set; }
    }
}