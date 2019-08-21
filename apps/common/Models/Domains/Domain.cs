using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amphora.Common.Attributes;
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
        public abstract bool IsValid(JObject o);
        public abstract Datum ToDatum(JObject o);
        public abstract List<DatumMemberAttribute> GetDatumMembers();

        public static List<Domain> GetAllDomains()
        {
            return new List<Domain>
            {
                new Dev.DefaultDomain(),
                new Agriculture.AgDomain()
            };
        }

        public static Domain GetDomain(DomainId id)
        {
            var allDomains = GetAllDomains();
            return allDomains.FirstOrDefault(d => string.Equals(id, d.Id));
        }

        protected List<DatumMemberAttribute> GetDatumMembers(Type t)
        {
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var results = new List<DatumMemberAttribute>();
            foreach(var p in properties)
            {
                var attribute = (DatumMemberAttribute) p.GetCustomAttribute(typeof(DatumMemberAttribute), true);
                if(attribute != null) results.Add(attribute);
            }

            return results;
        }

    }
}