using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Amphora.SharedUI.Models
{
    public class HeaderModel
    {
        private readonly Func<ClaimsPrincipal, Task<string>> profileUri;
        private readonly Func<ClaimsPrincipal, Task<bool>> isAdminGlobal;
        private static Uri defaultWebAppUri = new Uri("https://app.amphoradata.com");
        public string RegisterUrl { get; }
        public string ReturnUrl { get; }
        public Uri WebAppUri { get; }

        public HeaderModel(Func<ClaimsPrincipal, Task<string>> profileUri,
                           Func<ClaimsPrincipal, Task<bool>> isAdminGlobal,
                           string registerUrl,
                           string returnUrl = null,
                           Uri webAppUri = null)
        {
            this.profileUri = profileUri;
            this.isAdminGlobal = isAdminGlobal;
            RegisterUrl = registerUrl;
            ReturnUrl = returnUrl;
            WebAppUri = webAppUri ?? defaultWebAppUri;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                RegisterUrl += $"?returnUrl={returnUrl}";
            }
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return principal.Identity.IsAuthenticated;
        }

        public async Task<string> ProfileUriAsync(ClaimsPrincipal principal)
        {
            return profileUri != null ? await profileUri(principal) : "";
        }

        public async Task<bool> IsAdminGlobalAsync(ClaimsPrincipal principal)
        {
            return isAdminGlobal != null ? await isAdminGlobal(principal) : false;
        }
    }
}