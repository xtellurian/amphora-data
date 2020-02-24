using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Bogus;

namespace Amphora.Tests.Helpers
{
    public static class EntityLibrary
    {
        private static Random rnd = new Random();
        public static DetailedAmphora GetAmphoraDto(string orgId, string testName = null)
        {
            var a = new Faker<DetailedAmphora>()
               .StrictMode(false)
               .RuleFor(o => o.OrganisationId, f => orgId)
               .RuleFor(o => o.Labels, f => string.Join(',', f.Lorem.Words(5)))
               .RuleFor(o => o.Name, f => testName ?? f.Random.String2(1, 10))
               .RuleFor(o => o.Description, f => f.Lorem.Sentences())
               .RuleFor(o => o.Price, f => f.Random.Number(0, 99))
               .RuleFor(o => o.Lat, f => f.Random.Number(0, 89))
               .RuleFor(o => o.Lon, f => f.Random.Number(0, 89));

            return a.Generate();
        }

        public static AmphoraModel GetAmphoraModel(OrganisationModel org, bool isPublic = true)
        {
            var faker = new Faker<AmphoraModel>()
               .StrictMode(false)
               .RuleFor(_ => _.OrganisationId, f => org.Id)
               .RuleFor(_ => _.Name, f => f.Random.Word())
               .RuleFor(_ => _.Price, f => f.Random.Double(1, 100))
               .RuleFor(_ => _.Description, f => f.Lorem.Sentences())
               .RuleFor(_ => _.CreatedDate, f => f.Date.Past())
               .RuleFor(_ => _.IsPublic, f => isPublic)
               .RuleFor(_ => _.GeoLocation, f => new GeoLocation(f.Address.Longitude(), f.Address.Latitude()));

            return faker.Generate();
        }

        public static SignalV2 GetSignalV2(string property = null, string valueType = "Numeric")
        {
            property ??= "a" + Guid.NewGuid().ToString().Replace("-", "");
            return new SignalV2(property, valueType);
        }

        public static Signal GetSignalDto(string property = null, string valueType = "Numeric")
        {
            property ??= "a" + Guid.NewGuid().ToString().Replace("-", "");
            var faker = new Faker("en");
            var meta = new Dictionary<string, string>
            {
                { "units", "mm" },
                { Guid.NewGuid().ToString(), faker.Hacker.Phrase() },
                { Guid.NewGuid().ToString(), faker.Hacker.Phrase() },
            };

            return new Signal
            {
                Property = property,
                ValueType = valueType,
                Attributes = meta
            };
        }

        public static AmphoraModel GetInvalidAmphora(string id = null)
        {
            return new AmphoraModel("", null, -1 * rnd.Next(0, 99), null, null, null)
            {
                Id = id,
            };
        }

        public static Organisation GetOrganisationDto([CallerMemberName] string testName = "")
        {
            var org = new Faker<Organisation>()
               .StrictMode(false)
               .RuleFor(o => o.Name, f => f.Company.CompanyName())
               .RuleFor(o => o.About, f => testName + f.Lorem.Sentences())
               .RuleFor(o => o.WebsiteUrl, f => f.Internet.Url())
               .RuleFor(o => o.Address, f => f.Address.FullAddress());

            return org.Generate();
        }

        public static OrganisationModel GetOrganisationModel([CallerMemberName] string testName = "")
        {
            var testOrgs = new Faker<OrganisationModel>()
                // Ensure all properties have rules. By default, StrictMode is false
                .StrictMode(false)
                .RuleFor(o => o.Id, f => f.Random.Guid().ToString())
                .RuleFor(o => o.Address, f => f.Address.FullAddress());

            var org = testOrgs.Generate();
            return org;
        }

        public static SignalV2 GetV2Signal([CallerMemberName] string testName = "")
        {
            return new SignalV2(testName, SignalV2.Numeric);
        }
    }
}