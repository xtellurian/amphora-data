using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.TermsOfUse
{
    [CommonAuthorize]
    public class DetailModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;
        private readonly ITermsOfUseService termsOfUseService;
        private readonly ILogger<DetailModel> logger;

        public DetailModel(IOrganisationService organisationService,
                           IUserDataService userDataService,
                           ITermsOfUseService termsOfUseService,
                           ILogger<DetailModel> logger)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
            this.termsOfUseService = termsOfUseService;
            this.logger = logger;
        }

        public string ReturnUrl { get; set; }
        public OrganisationModel Organisation { get; private set; }
        public TermsOfUseModel TermsOfUse { get; private set; }
        public bool CanAccept { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string tncId, string returnUrl = null)
        {
            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await this.organisationService.ReadAsync(User, id);
            if (result.Succeeded && result.Entity.TermsOfUses.Any(t => t.Id == tncId))
            {
                this.Organisation = result.Entity;
                this.TermsOfUse = result.Entity.TermsOfUses.FirstOrDefault(t => t.Id == tncId);
                await SetCanAccept();
                return Page();
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("/Forbidden");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, string tncId, string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            var result = await this.organisationService.ReadAsync(User, id);

            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                this.TermsOfUse = result.Entity.TermsOfUses.FirstOrDefault(t => t.Id == tncId);
                await SetCanAccept(); // set Terms before calling this method
                var res = await termsOfUseService.AcceptAsync(User, TermsOfUse);
                if (res.Succeeded)
                {
                    return LocalRedirect(ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("/Index");
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

                if (userData.OrganisationId == Organisation.Id)
                {
                    // user in org can't accept
                    CanAccept = false;
                    return;
                }

                if (userData.Organisation.TermsOfUsesAccepted == null)
                {
                    // nothing accepted yet, so can accept.
                    CanAccept = true;
                    return;
                }

                if (userData.Organisation.TermsOfUsesAccepted.Any(t =>
                     t.TermsOfUseOrganisationId == Organisation.Id
                     && t.TermsOfUseId == this.TermsOfUse.Id)) // here is why Terms must not be null
                {
                    // org has already accepted
                    CanAccept = false;
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
