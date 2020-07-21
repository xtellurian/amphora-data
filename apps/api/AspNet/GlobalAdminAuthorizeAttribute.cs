using Amphora.Api.Services.Auth;

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