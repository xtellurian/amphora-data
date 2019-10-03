using System;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static AmphoraExtendedDto GetAmphoraDto(string orgId, string testName)
        {
            var lat = -32.868 + rnd.Next(2); // near sydney
            var lon = 150.2093 + rnd.Next(2);

            return new AmphoraExtendedDto()
            {
                OrganisationId = orgId,
                Description = DateTime.Now.ToString(),
                Price = rnd.Next(0, 99),
                Name = "test: " + testName,
                Lat = lat,
                Lon = lon
            };
        }

        public static AmphoraModel GetAmphoraModel(OrganisationModel org, string testName)
        {
            var lat = -32.868 + rnd.Next(2); // near sydney
            var lon = 150.2093 + rnd.Next(2);

            return new AmphoraModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                Organisation = org,
                Description = DateTime.Now.ToString(),
                Price = rnd.Next(0, 99),
                Name = "test: " + testName,
                GeoLocation = new GeoLocation(lon, lat)
            };
        }

        public static AmphoraModel GetInvalidAmphora(string id = null)
        {
            return new AmphoraModel()
            {
                Id = id,
                Description = null,
                Price = -1 * rnd.Next(0, 99),
                Name = ""
            };
        }

        public static OrganisationDto GetOrganisationDto(string testName)
        {
            return new OrganisationDto
            {
                Name = testName
            };
        }
        public static OrganisationModel GetOrganisationModel(string testName)
        {
            return new OrganisationModel
            {
                Name = testName
            };
        }

        public static SignalModel GetSignalModel(string testName)
        {
            return new SignalModel(testName, SignalModel.Numeric);
        }
        public static SignalDto GetSignalDto(string keyName)
        {
            return new SignalDto() { KeyName = keyName, ValueType = SignalModel.Numeric };
        }
    }
}