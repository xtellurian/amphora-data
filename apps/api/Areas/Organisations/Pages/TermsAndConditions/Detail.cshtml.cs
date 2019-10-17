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

        public DetailModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }
        public string ReturnUrl { get; set; }
        public OrganisationModel Organisation { get; private set; }
        public TermsAndConditionsModel TermsAndConditions { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name, string returnUrl = null)
        {

            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await this.organisationService.ReadAsync(User, id);
            if (result.Succeeded && result.Entity.TermsAndConditions.Any(t => t.Name == name))
            {
                this.Organisation = result.Entity;
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
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}