using System;
using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
            ttl = -1; // means no expiry
        }
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate {get; set; }

        public virtual bool IsValid() 
        {
            return  !string.IsNullOrEmpty(CreatedBy)
                && CreatedDate.HasValue;
        }
    }
}