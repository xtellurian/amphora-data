using System;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Maths
{
    public static class Haversine
    {
        private const double Radius = 6371;

        /// <summary>
        /// Returns the distance in kilometers of any two
        /// latitude / longitude points.
        /// </summary>
        /// <returns>The great circle distance, in km</returns>
        public static double Distance(GeoLocation pos1, GeoLocation pos2)
        {
            double dLat = ToRadians(pos2.Lat() - pos1.Lat());
            double dLon = ToRadians(pos2.Lon() - pos1.Lon());

            double a = Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(pos1.Lat())) * Math.Cos(ToRadians(pos2.Lat())) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = Radius * c;

            return d;
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        private static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}