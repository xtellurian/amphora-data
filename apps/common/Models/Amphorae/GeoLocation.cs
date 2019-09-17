using Newtonsoft.Json;

namespace Amphora.Common.Models.Amphorae
{
    public class GeoLocation
    {
        public GeoLocation() { }
        public GeoLocation(double lon, double lat)
        {
            Type = "Point";
            Coordinates = new double[2] { lon, lat };
        }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        public double? Lat() => Coordinates?[1];
        public double? Lon() => Coordinates?[0];


    }
    //{"type" : "Point", "coordinates": [long], [lat]
}
// https://docs.microsoft.com/en-us/rest/api/searchservice/data-type-map-for-indexers-in-azure-search#bkmk_json_search
