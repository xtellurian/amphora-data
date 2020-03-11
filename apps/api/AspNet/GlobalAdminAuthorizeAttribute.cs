using Amphora.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Amphora.Api.AspNet
{
    internal class GlobalAdminAuthorizeAttribute : CommonAuthorizeAttribute
    {
        public GlobalAdminAuthorizeAttribute()
        {
            Policy = GlobalAdminRequirement.GlobalAdminPolicyName;
        }
    }
}