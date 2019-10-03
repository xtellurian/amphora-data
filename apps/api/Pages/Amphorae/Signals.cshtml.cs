using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Newtonsoft.Json;

namespace Amphora.Api.Pages.Amphorae
{
    public class SignalsModel : AmphoraPageModel
    {
        private readonly ITsiService tsiService;

        public SignalsModel(IAmphoraeService amphoraeService, ITsiService tsiService) : base(amphoraeService)
        {
            this.tsiService = tsiService;
        }

        public string QueryResponse { get; set; }

        public override async Task<IActionResult> OnGetAsync(string id)
        {
            var result = await base.OnGetAsync(id);

            if (Amphora != null)
            {
                // do somerthign
            }

            QueryResponse = await GetQueryResponse();


            return result;

        }

        private async Task<string> GetQueryResponse()
        {
            var response = new List<QueryResultPage>();

            foreach (var signal in Amphora.Signals)
            {
                var r = await tsiService.FullSet(
                        Amphora.Id,
                        signal.Signal.KeyName,
                        DateTime.UtcNow.AddDays(-7),
                        DateTime.UtcNow
                    );

                response.Add(r);
            }
            return JsonConvert.SerializeObject(response);

        }
    }
}


