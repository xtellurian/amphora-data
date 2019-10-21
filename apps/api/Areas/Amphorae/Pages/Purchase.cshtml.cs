using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class PurchaseModel : PageModel
    {
        private readonly IPurchaseService purchaseService;
        private readonly IAmphoraeService amphoraeService;

        public PurchaseModel(IPurchaseService purchaseService, IAmphoraeService amphoraeService)
        {
            this.purchaseService = purchaseService;
            this.amphoraeService = amphoraeService;
        }

        public string Id { get; private set; }
        public bool Success { get; private set; }
        public AmphoraModel Amphora { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // TODO check if Terms and Condirtions are accepted.
            // if NOT, redirect to TnC, with returnUrl back here.
            // if YES, just purchase and continue
            this.Id = id;
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var hasAgreed = await purchaseService.HasAgreedToTermsAndConditionsAsync(User, result.Entity);
                if (hasAgreed)
                {
                    this.Success = true;
                    await purchaseService.PurchaseAmphora(User, result.Entity);
                    return Page();
                }
                else
                {
                    this.Success = false;
                    this.Amphora = result.Entity;
                    ModelState.AddModelError(string.Empty, "You must agree to the Amphora's terms and conditions");
                    return Page();
                }
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}
