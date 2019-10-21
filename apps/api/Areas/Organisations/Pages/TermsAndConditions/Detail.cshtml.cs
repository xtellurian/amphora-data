using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.TermsAndConditions
{
    public class DetailModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserService userService;

        public DetailModel(IOrganisationService organisationService, IUserService userService)
        {
            this.organisationService = organisationService;
            this.userService = userService;
        }
        public string ReturnUrl { get; set; }
        public OrganisationModel Organisation { get; private set; }
        public TermsAndConditionsModel TermsAndConditions { get; private set; }
        public bool CanAccept { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string name, string returnUrl = null)
        {

            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await this.organisationService.ReadAsync(User, id);
            if (result.Succeeded && result.Entity.TermsAndConditions.Any(t => t.Name == name))
            {
                this.Organisation = result.Entity;
                await SetCanAccept();
                this.TermsAndConditions = result.Entity.TermsAndConditions.FirstOrDefault(t => t.Name == name);
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

        public async Task<IActionResult> OnPostAsync(string id, string name, string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            var result = await this.organisationService.ReadAsync(User, id);

            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                await SetCanAccept();
                this.TermsAndConditions = result.Entity.TermsAndConditions.FirstOrDefault(t => t.Name == name);
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
                && t.TermsAndConditionsId == this.TermsAndConditions.Name))
            {
                // org has already accepted
                CanAccept = false;
                return;
            }
            CanAccept = true;
        }
    }
}