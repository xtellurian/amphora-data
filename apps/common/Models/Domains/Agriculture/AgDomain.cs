using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Amphora.Common.Models.Domains.Agriculture
{
    public class AgDomain : Domain
    {
        public override DomainId Id => DomainId.Agriculture;

        public override string Name => "Agriculture";

        public override SortedDictionary<string, Type> DatumColumns =>
             new SortedDictionary<string, Type>(
                typeof(AgDatum)
                .GetProperties()
                .ToDictionary(x => Char.ToLowerInvariant(x.Name[0]) + x.Name.Substring(1), y => y.PropertyType) // camelCase
            );

        public override bool IsValid(JObject o)
        {
            var datum = ToDatum(o);
            return datum.IsValid();
        }

        public override Datum ToDatum(JObject o)
        {
            return o.ToObject<AgDatum>();
        }
    }
}