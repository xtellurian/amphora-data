using System;
using System.Runtime.CompilerServices;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Signals;
using Bogus;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static AmphoraExtendedDto GetAmphoraDto(string orgId, string testName = null)
        {
            var a = new Faker<AmphoraExtendedDto>()
               .StrictMode(false)
               .RuleFor(o => o.OrganisationId, f => orgId)
               .RuleFor(o => o.Name, f => testName ?? f.Random.String2(1, 10))
               .RuleFor(o => o.Description, f => f.Random.String2(1, 10))
               .RuleFor(o => o.Price, f => f.Random.Number(0, 99))
               .RuleFor(o => o.Lat, f => f.Random.Number(0, 89))
               .RuleFor(o => o.Lon, f => f.Random.Number(0, 89));

            return a.Generate();
        }

        public static AmphoraModel GetAmphoraModel(OrganisationModel org, string testName, bool isPublic = true)
        {
            var lat = -32.868 + rnd.Next(2); // near sydney
            var lon = 150.2093 + rnd.Next(2);

            return new AmphoraModel("test: " + testName, DateTime.Now.ToString(), rnd.Next(0, 99), org.Id, null, null)
            {
                Id = System.Guid.NewGuid().ToString(),
                Organisation = org,
                GeoLocation = new GeoLocation(lon, lat),
                IsPublic = isPublic
            };
        }

        public static AmphoraModel GetInvalidAmphora(string id = null)
        {
            return new AmphoraModel("", null, -1 * rnd.Next(0, 99), null, null, null)
            {
                Id = id,
            };
        }

        public static OrganisationDto GetOrganisationDto([CallerMemberName] string testName = null)
        {
            var org = new Faker<OrganisationDto>()
               .StrictMode(false)
               .RuleFor(o => o.Name, f => f.Random.String2(1, 10))
               .RuleFor(o => o.About, f => testName ?? f.Random.String2(1, 10))
               .RuleFor(o => o.WebsiteUrl, f => f.Random.String2(1, 10))
               .RuleFor(o => o.Address, f => f.Address.FullAddress());

            return org.Generate();
        }

        public static OrganisationModel GetOrganisationModel([CallerMemberName] string testName = "")
        {
            var testOrgs = new Faker<OrganisationModel>()
                // Ensure all properties have rules. By default, StrictMode is false
                // Set a global policy by using Faker.DefaultStrictMode
                .StrictMode(false)
                .CustomInstantiator(f => new OrganisationModel(testName, f.Random.String2(10), f.Random.String2(10), f.Address.FullAddress()))
                // OrderId is deterministic
                // Pick some fruit from a basket
                .RuleFor(o => o.Id, f => f.Random.Guid().ToString())
                .RuleFor(o => o.Address, f => f.Random.String2(1, 10));
            // A nullable int? with 80% probability of being null.
            // The .OrNull extension is in the Bogus.Extensions namespace.;

            var org = testOrgs.Generate();
            return org;
        }

        public static SignalModel GetSignalModel(string testName)
        {
            return new SignalModel(testName, SignalModel.Numeric);
        }

        public static SignalDto GetSignalDto(string property)
        {
            return new SignalDto() { Property = property, ValueType = SignalModel.Numeric };
        }
    }
}