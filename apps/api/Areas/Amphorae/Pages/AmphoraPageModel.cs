using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Pages;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public abstract class AmphoraPageModel : PageModelBase
    {
        protected readonly IAmphoraeService amphoraeService;

        public AmphoraPageModel(IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        protected Error Error { get; set; } = null;

        // [BindProperty] // this shouldn't be bound, as it messes with model validation, and shouldn't be sent by the client
        public AmphoraModel Amphora { get; set; }
        public EntityOperationResult<AmphoraModel> Result { get; private set; }

        public virtual async Task LoadAmphoraAsync(string id)
        {
            if (id == null)
            {
                Error = new Error("Amphora Id cannot be empty");
                return;
            }

            this.Result = await amphoraeService.ReadAsync(User, id, true);
            if (Result.Succeeded)
            {
                this.Amphora = Result.Entity;
            }
            else
            {
                Error = new Error($"Amphora({id}) not found");
            }
        }

        public IActionResult OnReturnPage()
        {
            if (Amphora == null) { return RedirectToPage("/Index", new { area = "Amphorae" }); }
            if (Result.WasForbidden)
            {
                return RedirectToPage("/Forbidden");
            }
            else if (Result.Succeeded)
            {
                if (Amphora == null)
                {
                    return RedirectToPage("/NotFound", new { area = "Amphorae" });
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("/Index", new { area = "Amphorae" });
            }
        }
    }
}
