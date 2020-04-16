using System;
using Amphora.Common.Contracts;
using Newtonsoft.Json;

namespace Amphora.Common.Models
{
    public abstract class EntityBase : IEntity, ITtl
    {
        public EntityBase()
        {
            ttl = -1; // means no expiry
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the entity Id.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets Time to Live, in seconds.
        /// Defaults to -1 (doesn't expire)
        /// </summary>
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; } = -1; // don't expire

        /// <summary>
        /// Gets or sets soft delete option.
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets dateTime created in UTC.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets dateTime last modified in UTC.
        /// </summary>
        public DateTimeOffset? LastModified { get; set; } = DateTime.UtcNow;
    }
}