using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class DeleteModel: AmphoraPageModel
    {

        public DeleteModel(IAmphoraeService amphoraeService): base(amphoraeService)
        {
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            return base.OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var readResult = await amphoraeService.ReadAsync(User, id);
            if(readResult.Succeeded)
            {
                var deleteResult = await amphoraeService.DeleteAsync(User, readResult.Entity);
                if(deleteResult.Succeeded)
                {
                    return RedirectToPage("./Index");
                }
                else if(deleteResult.WasForbidden)
                {
                    return RedirectToPage("./Forbidden");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, deleteResult.Message);
                    return Page();
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, readResult.Message);
                return Page();
            }
        }
    }
}