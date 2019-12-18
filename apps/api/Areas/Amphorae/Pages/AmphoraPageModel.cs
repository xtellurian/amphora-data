using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public abstract class AmphoraPageModel: PageModel
    {
        protected readonly IAmphoraeService amphoraeService;

        public AmphoraPageModel(IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        [BindProperty]
        public AmphoraModel Amphora { get; set; }
        public EntityOperationResult<AmphoraModel> Result { get; private set; }

        public virtual async Task LoadAmphoraAsync(string id)
        {
            if(id == null) return;
            this.Result = await amphoraeService.ReadAsync(User, id, true);
            if(Result.Succeeded) this.Amphora = Result.Entity;
            else Amphora = null;;
        }


        public IActionResult OnReturnPage()
        {
            if (Amphora == null) return RedirectToPage("./Index");
            if (Result.WasForbidden)
            {
                return RedirectToPage("Amphorae/Forbidden");
            }

            else if (Result.Succeeded)
            {
                if (Amphora == null)
                {
                    return RedirectToPage("Amphorae/NotFound");
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("Amphorae/Index");
            }
        }
    }
}