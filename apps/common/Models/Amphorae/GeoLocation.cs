using System;
using Newtonsoft.Json;

namespace Amphora.Common.Models.Amphorae
{
    public class GeoLocation
    {
        public GeoLocation(double lon, double lat)
        {
            Type = "Point";
            if (lat > 90 || lat < -90) { throw new ArgumentException("A latitude coordinate must be a value between -90.0 and +90.0 degrees."); }
            if (lon > 180 || lon < -180) { throw new ArgumentException("A longitude coordinate must be a value between -180.0 and +180.0 degrees."); }
            Coordinates = new double[2] { lon, lat };
        }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        public double? Lat() => Coordinates?[1];
        public double? Lon() => Coordinates?[0];
    }

    // {"type" : "Point", "coordinates": [long], [lat]
}

// https://docs.microsoft.com/en-us/rest/api/searchservice/data-type-map-for-indexers-in-azure-search#bkmk_json_search
