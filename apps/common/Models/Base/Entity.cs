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
        /// <summary>
        /// The entity Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time to Live, in Seconds
        /// </summary>
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; } = -1; // don't expire
        /// <summary>
        /// Soft delete option.
        /// </summary>
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// DateTime created in UTC
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

    }
}