using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amphora.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Amphora.Common.Models.Domains.Dev
{
    public class DefaultDomain : Domain
    {

        public override string Name => "Default Domain";

        public override DomainId Id => DomainId.Dev;

        public override List<DatumMemberAttribute> GetDatumMembers()
        {
            return base.GetDatumMembers(typeof(DefaultDatum));
        }

        public override bool IsValid(JObject o)
        {
            var datum = ToDatum(o);
            return datum.IsValid();
        }

        public override Datum ToDatum(JObject o)
        {
            return o?.ToObject<DefaultDatum>();
        }

        public class DefaultDatum : Datum
        {

            [JsonProperty(Required = Required.Always)]
            [DatumMember("id", "string")]
            public string Id { get; set; }
            [JsonProperty(Required = Required.Always)]
            [DatumMember("value", "double", "any", "Whatever you like")]
            public double Value { get; set; }

        }
    }
}