using NGeoHash;

namespace Amphora.Api.Extensions
{
    public static class AmphoraModelExtensions
    {
        public static double? GetLat(this Amphora.Common.Models.AmphoraModel amphora)
        {
            if(string.IsNullOrEmpty(amphora?.GeoHash)) return null;
            return GeoHash.Decode(amphora.GeoHash).Coordinates.Lat;
        }
        public static double? GetLon(this Amphora.Common.Models.AmphoraModel amphora)
        {
            if(string.IsNullOrEmpty(amphora?.GeoHash)) return null;
            return GeoHash.Decode(amphora.GeoHash).Coordinates.Lon;
        }
    }
}