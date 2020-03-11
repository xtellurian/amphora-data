using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Amphora.SharedUI.Models
{
    public class HeaderModel
    {
        private readonly Func<ClaimsPrincipal, Task<string>> profileUri;
        private readonly Func<ClaimsPrincipal, Task<bool>> isAdminGlobal;

        public string RegisterUrl { get; }

        public HeaderModel(Func<ClaimsPrincipal, Task<string>> profileUri, Func<ClaimsPrincipal, Task<bool>> isAdminGlobal, string registerUrl)
        {
            this.profileUri = profileUri;
            this.isAdminGlobal = isAdminGlobal;
            RegisterUrl = registerUrl;
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