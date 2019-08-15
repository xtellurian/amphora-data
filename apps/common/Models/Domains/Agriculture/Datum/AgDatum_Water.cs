namespace Amphora.Common.Models.Domains.Agriculture
{
    public partial class AgDatum : Datum
    {
        public float? Discharge { get; set; }
        public float? Rainfall{ get; set; }
        public float? StoragePercent { get; set; }
        public float? Volume { get; set; }
        public float? WaterLevel { get; set; }
    }
}