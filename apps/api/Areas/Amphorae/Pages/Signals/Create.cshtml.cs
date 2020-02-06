using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class CreateModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;
        private readonly IOptionsMonitor<SignalOptions> options;

        public CreateModel(IAmphoraeService amphoraeService, ISignalService signalService, IOptionsMonitor<SignalOptions> options) : base(amphoraeService)
        {
            this.signalService = signalService;
            this.options = options;
        }

        [BindProperty]
        public Signal Signal { get; set; }
        public SelectList Options { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            Options = new SelectList(SignalV2.Options);
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                Amphora.EnsureV2Signals();
                Amphora.V2Signals.Add(new SignalV2(Signal.Property, Signal.ValueType));
                var res = await amphoraeService.UpdateAsync(User, Amphora);
                if (res.Succeeded)
                {
                    return RedirectToPage("./Index", new { Id = Amphora.Id });
                }
                else if (res.WasForbidden)
                {
                    return RedirectToPage("Amphorae/Forbidden");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Result.Message);
                    return Page();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}