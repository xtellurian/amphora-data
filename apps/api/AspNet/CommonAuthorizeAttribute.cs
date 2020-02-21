using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Amphora.Api.AspNet
{
    internal class CommonAuthorizeAttribute : AuthorizeAttribute
    {
        public CommonAuthorizeAttribute()
        {
            AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
        }
    }
}