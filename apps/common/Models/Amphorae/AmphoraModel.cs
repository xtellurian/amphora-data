using Amphora.Common.Extensions;
using Amphora.Common.Models.Domains;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Amphora.Common.Models.Amphorae
{
    public class AmphoraModel : Entity
    {
        public string AmphoraId { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public double? Price { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DomainId DomainId { get; set; }

        public override void SetIds()
        {
            this.AmphoraId = System.Guid.NewGuid().ToString();
            this.Id = this.AmphoraId.AsQualifiedId(typeof(AmphoraModel));
            this.EntityType = typeof(AmphoraModel).GetEntityPrefix();
        }
    }
}