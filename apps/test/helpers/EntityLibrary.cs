using System;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using NGeoHash;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static Amphora.Common.Models.Amphora GetAmphora(string id = null, string description = null)
        {
            var geoHash = GeoHash.Encode(rnd.Next(0, 180), rnd.Next(0, 180));
            return new Amphora.Common.Models.Amphora()
            {
                Id = id,
                Description = description ?? "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                Title = "Valid Amphora - title",
                GeoHash = geoHash
            };
        }

        public static Amphora.Common.Models.Amphora GetInvalidAmphora(string id = null)
        {
            return new Amphora.Common.Models.Amphora()
            {
                Id = id,
                Description = null,
                Price = -1 * rnd.Next(0, 99),
                Title = ""
            };
        }

        public static Organisation GetOrganisation()
        {
            return new Organisation
            {
                Name = "Test Org",
                InviteCode = "Welcome to the world"
            };
        }
    }
}