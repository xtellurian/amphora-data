using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace Amphora.Common.Models.Domains
{
    public class DevDomain : Domain
    {

        public override string Name => "Development Domain";

        public override DomainId Id => DomainId.Dev;

        public override JSchema GetDomainSchema()
        {
            var generator = new JSchemaGenerator();
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return generator.Generate(typeof(DevDatum));
        }

        public override bool IsValid(JObject o)
        {
            return o.IsValid(this.GetDomainSchema());
        }

        public override Datum ToDatum(JObject o)
        {
            return o?.ToObject<DevDatum>();
        }

        public class DevDatum: Datum
        {
            
            [JsonProperty(Required = Required.Always)]
            public string Id { get; set; }
            [JsonProperty(Required = Required.Always)]
            public double Value { get; set; }

        }
    }
}