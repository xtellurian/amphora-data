using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Amphora.Common.Models.Domains
{
    public class DevDomain : Domain
    {

        public override string Name => "Development Domain";

        public override DomainId Id => DomainId.Dev;

        public override SortedDictionary<string, Type> DatumColumns =>

            new SortedDictionary<string, Type>(
                typeof(DevDatum)
                .GetProperties()
                .ToDictionary(x => x.Name, y => y.PropertyType)
            );

        public override bool IsValid(JObject o)
        {
            var datum = ToDatum(o);
            return datum.IsValid();
        }

        public override Datum ToDatum(JObject o)
        {
            return o?.ToObject<DevDatum>();
        }

        public class DevDatum : Datum
        {

            [JsonProperty(Required = Required.Always)]
            public string Id { get; set; }
            [JsonProperty(Required = Required.Always)]
            public double Value { get; set; }

        }
    }
}