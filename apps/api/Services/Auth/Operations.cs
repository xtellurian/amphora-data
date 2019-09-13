using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Amphora.Api.Services.Auth
{
    public static class Operations
    {
        public static IAuthorizationRequirement Create =
            new AuthorizationRequirement { MinimumLevel = AccessLevels.Administer };
        public static IAuthorizationRequirement Read =
            new AuthorizationRequirement { MinimumLevel = AccessLevels.Read };
        public static IAuthorizationRequirement Update =
            new AuthorizationRequirement { MinimumLevel = AccessLevels.Update };
        public static IAuthorizationRequirement Delete =
            new AuthorizationRequirement { MinimumLevel = AccessLevels.Administer };
    }
}