using Microsoft.AspNetCore.Authorization;

namespace Amphora.Api.Services.Auth
{
    public class GlobalAdminRequirement : IAuthorizationRequirement
    {
        public static string GlobalAdminPolicyName => "GlobalAdmin";
    }
}