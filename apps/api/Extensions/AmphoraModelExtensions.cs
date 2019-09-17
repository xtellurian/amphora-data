using Amphora.Common.Models.Amphorae;
using NGeoHash;

namespace Amphora.Api.Extensions
{
    public static class AmphoraModelExtensions
    {
        public static double? GetLat(this AmphoraExtendedModel amphora)
        {
            if(string.IsNullOrEmpty(amphora?.GeoHash)) return null;
            return GeoHash.Decode(amphora.GeoHash).Coordinates.Lat;
        }
        public static double? GetLon(this AmphoraExtendedModel amphora)
        {
            if(string.IsNullOrEmpty(amphora?.GeoHash)) return null;
            return GeoHash.Decode(amphora.GeoHash).Coordinates.Lon;
        }
    }
}