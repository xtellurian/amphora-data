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
            SetAuthenticationSchemes();
        }

        public CommonAuthorizeAttribute(string policy) : base(policy)
        {
            SetAuthenticationSchemes();
        }

        private void SetAuthenticationSchemes()
        {
            AuthenticationSchemes ??= "";
            // the order matters!
            AuthenticationSchemes += $"{JwtBearerDefaults.AuthenticationScheme},{CookieSchemeName},{OidcSchemeName}";
        }
    }
}