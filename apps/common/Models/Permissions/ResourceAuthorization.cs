using System;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models
{
    public class ResourceAuthorization
    {
        public ResourceAuthorization() { /* Empty Constructor */}
        public ResourceAuthorization(string userId, IEntity target, string permission)
        {
            this.UserId = userId;
            this.TargetResourceId = target.Id;
            this.ResourcePermission = permission;
        }
        public string TargetResourceId { get; set; }
        public string UserId { get; set; }
        public string ResourcePermission { get; set; }

    }
}