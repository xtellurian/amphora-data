using System;
using Amphora.Common.Extensions;

namespace Amphora.Common.Models.UserData
{
    public class UserDataModel : Entity
    {
        public string UserDataId {get; set; }
        public override void SetIds()
        {
            this.UserDataId = Guid.NewGuid().ToString();
            this.Id = this.UserDataId.AsQualifiedId<UserDataModel>();
            this.EntityType = typeof(UserDataModel).GetEntityPrefix();
        }
    }
}