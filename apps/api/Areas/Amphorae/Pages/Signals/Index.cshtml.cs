using System;
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
    public class IndexModel : AmphoraPageModel
    {
        private readonly ISignalService signalService;

        public IndexModel(IAmphoraeService amphoraeService, ISignalService signalService) : base(amphoraeService)
        {
            this.signalService = signalService;
        }

        public string QueryResponse { get; set; }
        public IEnumerable<SignalModel> Signals { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                this.Signals = this.Amphora.Signals.Select(s => s.Signal);
                QueryResponse = await GetQueryResponse();
            }

            return OnReturnPage();

        }

        private async Task<string> GetQueryResponse()
        {
            // should be a list of query result (for javascript formatting tsi.js)
            var res = await signalService.GetTsiSignalsAsync(User, Amphora);

            return JsonConvert.SerializeObject(res);
        }
    }
}


