using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models
{
    public abstract class Entity : IEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string OrganisationId { get; set; }
    }
}