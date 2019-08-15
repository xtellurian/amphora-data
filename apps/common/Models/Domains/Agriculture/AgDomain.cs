using System;
using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Attributes;
using Newtonsoft.Json.Linq;

namespace Amphora.Common.Models.Domains.Agriculture
{
    public class AgDomain : Domain
    {
        public override DomainId Id => DomainId.Agriculture;

        public override string Name => "Agriculture";

        public override List<DatumMemberAttribute> GetDatumMembers()
        {
            return this.GetDatumMembers(typeof(AgDatum));
        }

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