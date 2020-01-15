using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.DataRequests;
using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.DataRequests
{
    [Authorize]
    public class CreatePageModel : PageModel
    {
        private readonly IPermissionedEntityStore<DataRequestModel> store;
        [BindProperty]
        public CreateDataRequest DataRequest { get; set; }
        public CreatePageModel(IPermissionedEntityStore<DataRequestModel> store)
        {
            this.store = store;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var model = DataRequest?.ToEntity();
                if (model != null)
                {
                    var result = await store.CreateAsync(User, model);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("./Detail", new { id = result.Entity.Id });
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Message);
                        return Page();
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Something went wrong.");
                    return Page();
                }
            }
            else
            {
                // there are errors in the model
                return Page();
            }
        }
    }
}