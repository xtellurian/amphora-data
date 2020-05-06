using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Amphorae.Pages.TermsOfUse
{
    [CommonAuthorize]
    public class DetailModel : PageModel
    {
        private readonly IUserDataService userDataService;
        private readonly ITermsOfUseService termsOfUseService;
        private readonly ILogger<DetailModel> logger;

        public DetailModel(IUserDataService userDataService,
                           ITermsOfUseService termsOfUseService,
                           ILogger<DetailModel> logger)
        {
            this.userDataService = userDataService;
            this.termsOfUseService = termsOfUseService;
            this.logger = logger;
        }

        public string ReturnUrl { get; set; }
        public TermsOfUseModel TermsOfUse { get; private set; }
        public bool CanAccept { get; set; }
        public bool AlreadyAccepted { get; private set; } = false;

        public async Task<IActionResult> OnGetAsync(string id, string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            var readRes = await termsOfUseService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                await LoadProperties(readRes);
                return Page();
            }
            else if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        private async Task LoadProperties(EntityOperationResult<TermsOfUseModel> readRes)
        {
            this.TermsOfUse = readRes.Entity;
            await SetCanAccept();
        }

        public async Task<IActionResult> OnPostAsync(string id, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var readRes = await termsOfUseService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                await LoadProperties(readRes);
                var res = await termsOfUseService.AcceptAsync(User, TermsOfUse);
                if (res.Succeeded)
                {
                    if (string.IsNullOrEmpty(ReturnUrl))
                    {
                        await LoadProperties(readRes);
                        return Page();
                    }
                    else
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                    return Page();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ReturnUrl))
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    return LocalRedirect(ReturnUrl);
                }
            }
        }

        private async Task SetCanAccept()
        {
            // set TermsOfUse before calling this method
            if (this.TermsOfUse == null) { throw new System.ApplicationException("Terms must not be null when calling this method."); }
            try
            {
                var userReadRes = await userDataService.ReadAsync(User);
                if (!userReadRes.Succeeded)
                {
                    throw new System.ApplicationException("Coudn't read user data");
                }

                var userData = userReadRes.Entity;

                if (userData.OrganisationId == TermsOfUse.OrganisationId)
                {
                    // user in org can't accept
                    CanAccept = false;
                    AlreadyAccepted = true;
                    return;
                }

                if (userData.Organisation.TermsOfUsesAccepted == null)
                {
                    // nothing accepted yet, so can accept.
                    CanAccept = true;
                    return;
                }

                // here is why Terms must not be null
                if (userData.Organisation.TermsOfUsesAccepted.Any(t => t.TermsOfUseId == this.TermsOfUse.Id))
                {
                    // org has already accepted
                    CanAccept = false;
                    AlreadyAccepted = true;
                    return;
                }

                CanAccept = true;
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}
