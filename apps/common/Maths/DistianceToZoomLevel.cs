using System;

namespace Amphora.Common.Maths
{
    public static class DistanceToZoomLevel
    {
        private const int MetersPerTileSideAtZeroZoom = 40075008;
        private const int MaxZoom = 22;
        private const int MinZoom = 0; // the whole world
        private static double MetersPerTileSize(long zoom) => MetersPerTileSideAtZeroZoom / Math.Pow(2, zoom);

        /// <summary>
        /// Returns the zoom level that fits this distance inside 1 tile
        /// </summary>
        /// <param name=distance>Distance, in meters</param>
        public static int DistanceInMetersToZoomLevel(double distance)
        {
            // start big, and zoom in, until meters per side is less than the distance
            for (int z = 0; z <= MaxZoom; z++)
            {
                try
                {
                    // wait until the meters per tile is smaller than the distance we want to fit
                    var mpt = MetersPerTileSize(z);
                    if (mpt < distance)
                    {
                        return z - 1; // now go back up 1 zoom level so we're sure it fits
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    System.Console.WriteLine($"Error caluclating zoom for d={distance}m, z={z}");
                }
            }

            return MinZoom;
        }
    }
}