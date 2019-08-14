using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Amphora.Common.Models.Domains
{
    public abstract class Domain
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract DomainId Id { get; }
        public abstract string Name { get; }
        public abstract SortedDictionary<string, Type> DatumColumns { get; }
        public abstract bool IsValid(JObject o);
        public abstract Datum ToDatum(JObject o);

        public static List<Domain> GetAllDomains()
        {
            return new List<Domain>
            {
                new Dev.DevDomain(),
                new Agriculture.AgDomain()
            };
        }

        public static Domain GetDomain(DomainId id)
        {
            var allDomains = GetAllDomains();
            return allDomains.FirstOrDefault(d => string.Equals(id, d.Id));
        }

    }
}