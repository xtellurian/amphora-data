using System;

namespace Amphora.Api.Models.Dtos
{
    public abstract class Entity
    {
        public virtual string Id { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}