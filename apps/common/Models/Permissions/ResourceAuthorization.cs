using System;
using Amphora.Common.Contracts;

namespace Amphora.Common.Models.Permissions
{
    public class ResourceAuthorization
    {
        public ResourceAuthorization() { /* Empty Constructor */}
        public ResourceAuthorization(string userId, string userName, IEntity target, AccessLevels accessLevel)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.TargetEntityId = target.Id;
            this.AccessLevel = accessLevel;
        }
        public string TargetEntityId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public AccessLevels AccessLevel {get; set; }

    }
}