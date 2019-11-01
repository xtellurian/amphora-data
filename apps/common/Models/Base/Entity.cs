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
            CreatedDate = DateTime.UtcNow;
        }
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; } = -1; // don't expire
        public DateTimeOffset? CreatedDate {get; set; }

    }
}