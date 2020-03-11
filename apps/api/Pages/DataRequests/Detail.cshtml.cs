using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.DataRequests
{
    [CommonAuthorize]
    public class DetailPageModel : PageModel
    {
        private readonly IPermissionedEntityStore<DataRequestModel> store;

        public DetailPageModel(IPermissionedEntityStore<DataRequestModel> store)
        {
            this.store = store;
        }

        public DataRequestModel DataRequest { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var response = await store.ReadAsync(User, id);
            if (id != null && response.Succeeded)
            {
                this.DataRequest = response.Entity;
                return Page();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostVoteAsync(string id)
        {
            var response = await store.ReadAsync(User, id);
            if (id != null && response.Succeeded)
            {
                this.DataRequest = response.Entity;
                if (this.DataRequest.TryVote(response.User.Id))
                {
                    // update the model
                    var res = await store.UpdateAsync(User, this.DataRequest);
                    if (res.Succeeded)
                    {
                        this.DataRequest = res.Entity;
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, res.Message);
                    }
                }

                return RedirectToPage("./Detail", new { id = DataRequest.Id });
            }
            else
            {
                return NotFound();
            }
        }
    }
}