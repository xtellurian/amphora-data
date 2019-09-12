using System;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using Amphora.Common.Models.Organisations;
using NGeoHash;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static Amphora.Common.Models.AmphoraModel GetAmphora(string orgId, string description = null)
        {
            var geoHash = GeoHash.Encode(rnd.Next(0, 180), rnd.Next(0, 180));
            return new Amphora.Common.Models.AmphoraModel()
            {
                Id = null,
                OrganisationId = orgId,
                Description = description ?? "Valid Amphora - description",
                Price = rnd.Next(0, 99),
                Name = "Valid Amphora - title",
                GeoHash = geoHash
            };
        }

        public static Amphora.Common.Models.AmphoraModel GetInvalidAmphora(string id = null)
        {
            return new Amphora.Common.Models.AmphoraModel()
            {
                Id = id,
                Description = null,
                Price = -1 * rnd.Next(0, 99),
                Name = ""
            };
        }

        public static OrganisationModel GetOrganisation()
        {
            return new OrganisationExtendedModel
            {
                Name =  Guid.NewGuid().ToString()
            };
        }
    }
}