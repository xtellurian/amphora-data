using System;
using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {

        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string OrganisationId { get; set; }
        public string EntityType {get; set; }
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate {get; set; }
        public string GetEntityId() => Id?.Split('|')[1]; // gets what's after the pipe - i.e. the id
        // should set both the Ids. Only use on create.
        public abstract void SetIds();

        public virtual bool IsValid() 
        {
            return !string.IsNullOrEmpty(OrganisationId)
                && !string.IsNullOrEmpty(CreatedBy)
                && CreatedDate.HasValue;
        }
    }
}