using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    public class AgreementModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;

        public AgreementModel(IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        public AmphoraModel Amphora { get; private set; }
        public string AgreementText => "This is a placeholder. The text here will be the specific license for using the data. It will be provided by the creator of the Amphora and may take into account data ownership, how the data can be used and any other topics that are may be needed";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToPage("./Index");
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else if (result.Succeeded)
            {
                this.Amphora = result.Entity;
                return Page();
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }


}