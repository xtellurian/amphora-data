using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [CommonAuthorize]
    public class PurchasePageModel : PageModel
    {
        private readonly IPurchaseService purchaseService;
        private readonly IAmphoraeService amphoraeService;

        private Random Rand => new Random();
        public string CelebrationImagePath => images[Rand.Next(0, images.Count)];
        private static List<string> images = new List<string>
        {
            "/images/stock/celebrate/undraw_Beer_celebration_cefj.svg",
            "/images/stock/celebrate/undraw_celebration_0jvk.svg",
            "/images/stock/celebrate/undraw_happy_feeling_slmw.svg",
            "/images/stock/celebrate/undraw_having_fun_iais.svg",
            "/images/stock/celebrate/undraw_joyride_hnno.svg",
            "/images/stock/celebrate/undraw_online_wishes_dlmr.svg"
        };

        public PurchasePageModel(IPurchaseService purchaseService, IAmphoraeService amphoraeService)
        {
            this.purchaseService = purchaseService;
            this.amphoraeService = amphoraeService;
        }

        public string Id { get; private set; }
        public bool Success { get; private set; }
        public AmphoraModel Amphora { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Id = id;
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var hasAgreed = await purchaseService.HasAgreedToTermsAndConditionsAsync(User, result.Entity);
                var canPurchase = await purchaseService.CanPurchaseAmphoraAsync(User, result.Entity);
                if (!canPurchase)
                {
                    return StatusCode(403);
                }

                if (hasAgreed)
                {
                    this.Success = true;
                    await purchaseService.PurchaseAmphoraAsync(User, result.Entity);
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
                return RedirectToPage("/Amphorae/Forbidden");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}
