using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.TermsAndConditions
{
    public class DetailModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserService userService;
        private readonly ILogger<DetailModel> logger;

        public DetailModel(IOrganisationService organisationService, IUserService userService, ILogger<DetailModel> logger)
        {
            this.organisationService = organisationService;
            this.userService = userService;
            this.logger = logger;
        }
        public string ReturnUrl { get; set; }
        public OrganisationModel Organisation { get; private set; }
        public TermsAndConditionsModel TermsAndConditions { get; private set; }
        public bool CanAccept { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string tncId, string returnUrl = null)
        {

            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await this.organisationService.ReadAsync(User, id);
            if (result.Succeeded && result.Entity.TermsAndConditions.Any(t => t.Id == tncId))
            {
                this.Organisation = result.Entity;
                this.TermsAndConditions = result.Entity.TermsAndConditions.FirstOrDefault(t => t.Id == tncId);
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
                this.TermsAndConditions = result.Entity.TermsAndConditions.FirstOrDefault(t => t.Id == tncId);
                await SetCanAccept(); // set TermsAndConditions before calling this method
                var res = await organisationService.AgreeToTermsAndConditions(User, TermsAndConditions);
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
            // set TermsAndConditions before calling this method
            if(this.TermsAndConditions == null) throw new System.Exception("Terms and Conditions must not be null when calling this method.");
            try
            {
                var user = await userService.ReadUserModelAsync(User);
                if(user.OrganisationId == Organisation.Id) 
                {
                    // user in org can't accept 
                    CanAccept = false;
                    return;
                }
                if(user.Organisation.TermsAndConditionsAccepted == null) 
                {
                    // nothing accepted yet, so can accept.
                    CanAccept = true;
                    return;
                }
                if(user.Organisation.TermsAndConditionsAccepted.Any(t => 
                    t.TermsAndConditionsOrganisationId == Organisation.Id 
                    && t.TermsAndConditionsId == this.TermsAndConditions.Id)) // here is why TermsAndConditions must not be null
                {
                    // org has already accepted
                    CanAccept = false;
                    return;
                }
                CanAccept = true;
            }
            catch(System.Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}