namespace Amphora.Common.Models.Domains.Agriculture
{
    public class AgDatum : Datum
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Rainfall_mm { get; set; }
    }
}