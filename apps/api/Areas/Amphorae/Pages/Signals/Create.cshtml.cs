using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

        public Dictionary<string, KeyValuePair<string, string>> Meta { get; set; } = new Dictionary<string, KeyValuePair<string, string>>();

        [BindProperty]
        public Signal Signal { get; set; }
        public SelectList Options { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            Meta = new Dictionary<string, KeyValuePair<string, string>>();
            Options = new SelectList(SignalV2.Options);
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id, [FromForm] string meta)
        {
            await LoadAmphoraAsync(id);
            Meta = new Dictionary<string, KeyValuePair<string, string>>();
            Options = new SelectList(SignalV2.Options);
            if (ModelState.IsValid)
            {
                if (Amphora != null)
                {
                    var signal = new SignalV2(Signal.Property, Signal.ValueType);

                    this.Meta = JsonConvert.DeserializeObject<Dictionary<string, KeyValuePair<string, string>>>(meta);
                    var dic = this.Meta?.ToChildDictionary();
                    signal.Attributes = new Common.Models.Amphorae.AttributeStore(dic);
                    string message;
                    if (Amphora.TryAddSignal(signal, out message))
                    {
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
                        this.ModelState.AddModelError(string.Empty, message);
                        return Page();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Page();
            }
        }
    }
}