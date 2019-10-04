using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    public class DetailModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public DetailModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        public List<SignalModel> Signals => new List<SignalModel>{Signal};
        public SignalModel Signal { get; private set; }
        public string QueryResponse { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string signalId)
        {
            await base.LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                Signal = this.Amphora.Signals?.FirstOrDefault(s => s.SignalId == signalId)?.Signal;
                QueryResponse = await GetQueryResponse();
            }


            return OnReturnPage();
        }

        private async Task<string> GetQueryResponse()
        {
            // should be a list of query result (for javascript formatting tsi.js)
            var q = await signalService.GetTsiSignalAsync(Amphora, Signal, true);
            var res = new List<QueryResultPage>()
            {
                q
            };
            // must be an array
            return JsonConvert.SerializeObject(res);
        }
    }
}