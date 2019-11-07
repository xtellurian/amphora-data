using System;

namespace Amphora.Api.Models.Dtos
{
    public abstract class EntityDto
    {
        public virtual string Id { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}