using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Amphora.Common.Models.Domains
{
    public abstract class Domain
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract bool IsValid(JObject o);
        public abstract Datum ToDatum(JObject o);
        public abstract JSchema GetDomainSchema();

        public static List<Domain> GetAllDomains()
        {
            return new List<Domain>
            {
                new DevDomain()
            };
        }

        public static Domain GetDomain(string id)
        {
            var allDomains = GetAllDomains();
            return allDomains.FirstOrDefault(d => string.Equals(id, d.Id ));
        }

    }
}