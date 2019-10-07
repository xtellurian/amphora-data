using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class PurchaseModel: PageModel
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

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Id = id;
            var result = await amphoraeService.ReadAsync(User, id);
            if(result.Succeeded)
            {
                this.Success = true;
                await purchaseService.PurchaseAmphora(User, result.Entity);
                return Page();
            }
            else if(result.WasForbidden)
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
