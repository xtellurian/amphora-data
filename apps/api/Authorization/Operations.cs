using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Amphora.Api.Authorization
{
    public static class Operations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = ResourcePermissions.Create };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = ResourcePermissions.Read };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = ResourcePermissions.Update };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = ResourcePermissions.Delete };
    }
}