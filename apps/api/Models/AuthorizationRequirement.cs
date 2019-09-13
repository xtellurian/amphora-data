using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Amphora.Api.Models
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public AccessLevels MinimumLevel { get; set; }
    }
}