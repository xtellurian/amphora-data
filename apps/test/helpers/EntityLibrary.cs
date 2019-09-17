using System;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using NGeoHash;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static AmphoraExtendedModel GetAmphora(string orgId, string testName)
        {
            var geoHash = GeoHash.Encode(rnd.Next(0, 180), rnd.Next(0, 180));
            return new AmphoraExtendedModel()
            {
                Id = null,
                OrganisationId = orgId,
                Description = DateTime.Now.ToString(),
                Price = rnd.Next(0, 99),
                Name = "test: " + testName,
                GeoHash = geoHash
            };
        }

        public static AmphoraModel GetInvalidAmphora(string id = null)
        {
            return new AmphoraExtendedModel()
            {
                Id = id,
                Description = null,
                Price = -1 * rnd.Next(0, 99),
                Name = ""
            };
        }

        public static OrganisationModel GetOrganisation(string testName)
        {
            return new OrganisationExtendedModel
            {
                Name =  testName
            };
        }
    }
}