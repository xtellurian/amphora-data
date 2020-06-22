using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Pages
{
    public class RegisterPageModel : PageModelBase
    {
        private readonly IOptionsMonitor<ExternalServices> externalServices;

        public RegisterPageModel(IOptionsMonitor<ExternalServices> externalServices)
        {
            this.externalServices = externalServices;
        }

        public IActionResult OnGet()
        {
            var protocol = Request.IsHttps ? "https" : "http";
            var returnUrl = $"{protocol}://{Request.Host}/Challenge";
            string registerUrl = $"{externalServices.CurrentValue.IdentityUri().ToStandardString()}/Register?returnUrl={returnUrl}";

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(registerUrl);
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}