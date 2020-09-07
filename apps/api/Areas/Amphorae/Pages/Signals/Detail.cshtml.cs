using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [CommonAuthorize]
    public class DetailModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public DetailModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        public List<SignalV2> Signals => new List<SignalV2> { Signal };
        public SignalV2 Signal { get; private set; }
        public IEnumerable<string> Values { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string signalId)
        {
            if (signalId == null)
            {
                return RedirectToPage("/Detail/Signals", new { id = id });
            }

            await LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                Signal = this.Amphora.V2Signals?.FirstOrDefault(s => s.Id == signalId);
                if (Signal.IsString)
                {
                    var uniqueValues = await signalService.GetUniqueValuesForStringProperties(User, Amphora);
                    if (uniqueValues.TryGetValue(Signal, out var values))
                    {
                        this.Values = values;
                    }
                }
            }

            return OnReturnPage();
        }
    }
}