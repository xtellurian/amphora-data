using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Amphora.Api.AspNet
{
    internal class CommonAuthorizeAttribute : AuthorizeAttribute
    {
        public static string CookieSchemeName => "Cookies";
        public static string OidcSchemeName => "oidc";
        public CommonAuthorizeAttribute() : base()
        {
            AuthenticationSchemes ??= "";
            // the order matters!
            AuthenticationSchemes += $"{JwtBearerDefaults.AuthenticationScheme},{CookieSchemeName},{OidcSchemeName}";
        }
    }
}