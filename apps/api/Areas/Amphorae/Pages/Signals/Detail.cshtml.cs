using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class DetailModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public DetailModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        public List<SignalModel> Signals => new List<SignalModel> { Signal };
        public SignalModel Signal { get; private set; }
        public string QueryResponse { get; private set; }
        public IEnumerable<string> Values { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string signalId)
        {
            if (signalId == null)
            {
                return RedirectToPage("./Index", new { id = id });
            }

            await LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                Signal = this.Amphora.Signals?.FirstOrDefault(s => s.SignalId == signalId)?.Signal;
                if (Signal.IsString)
                {
                    var uniqueValues = await signalService.GetUniqueValuesForStringProperties(User, Amphora);
                    if (uniqueValues.TryGetValue(Signal, out var values))
                    {
                        this.Values = values;
                    }
                }

                QueryResponse = await GetQueryResponse();
            }

            return OnReturnPage();
        }

        private async Task<string> GetQueryResponse()
        {
            // should be a list of query result (for javascript formatting tsi.js)
            var q = await signalService.GetTsiSignalAsync(User, Amphora, Signal, true);
            var res = new List<QueryResultPage>()
            {
                q
            };
            // must be an array
            return JsonConvert.SerializeObject(res);
        }
    }
}