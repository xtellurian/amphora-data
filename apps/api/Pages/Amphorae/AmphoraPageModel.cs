using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    public abstract class AmphoraPageModel: PageModel
    {
        protected readonly IAmphoraeService amphoraeService;

        public AmphoraPageModel(IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        [BindProperty]
        public AmphoraExtendedModel Amphora { get; set; }

        public virtual async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToPage("./Index");

            var result = await amphoraeService.ReadAsync<AmphoraExtendedModel>(User, id);
            if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else if (result.Succeeded)
            {
                Amphora = result.Entity;
                if (Amphora == null)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}