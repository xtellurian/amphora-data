using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class CreateModel : AmphoraPageModel
    {
        private readonly IOptionsMonitor<SignalOptions> options;

        public CreateModel(IAmphoraeService amphoraeService, IOptionsMonitor<SignalOptions> options) : base(amphoraeService)
        {
            this.options = options;
        }

        

        [BindProperty]
        public SignalDto Signal {get;set;}
        public SelectList Options { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            Options = new SelectList(SignalModel.Options);
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            var signalModel = new SignalModel(Signal.Property, Signal.ValueType);
            Amphora.AddSignal(signalModel, options.CurrentValue?.MaxSignals ?? 7);
            var res = await amphoraeService.UpdateAsync(User, Amphora);
            if(res.Succeeded)
            {
                return RedirectToPage("./Index", new {Id = Amphora.Id});
            }
            else if(res.WasForbidden)
            {
                return RedirectToPage("Amphorae/Forbidden");
            }
            else
            {
                ModelState.AddModelError(string.Empty, Result.Message);
                return Page();
            }
        }
    }
}