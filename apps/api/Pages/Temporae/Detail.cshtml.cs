using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeSeriesInsightsClient.Queries;

namespace Amphora.Api.Pages.Temporae
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> temporaEntityStore;
        private readonly IDataStore<Common.Models.Amphora, Datum> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public DetailModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> temporaEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, Datum> dataStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        public string QueryResponse {get; set; }

        [BindProperty]
        public Amphora.Common.Models.Amphora Tempora { get; set; }
        public Amphora.Common.Models.Domains.Domain Domain { get; set; }
        public string Token { get; set; } = "blank for now";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Tempora = await temporaEntityStore.ReadAsync(id);
            if (Tempora == null)
            {
                return RedirectToPage("/amphorae/index");
            }

            Domain = Common.Models.Domains.Domain.GetDomain(Tempora.DomainId);
            var response = new List<QueryResponse>();
            foreach(var member in Domain.GetDatumMembers())
            {
                // then we can do a thing
                if(string.Equals(member.Name, "t")) continue; // skip t // TODO remove hardcoding
                var r = await tsiService
                    .WeeklyAverageAsync(
                        Tempora.Id, 
                        member.Name, 
                        DateTime.UtcNow.AddDays(-365),
                        DateTime.UtcNow
                    );
                response.Add(r);
            }
            QueryResponse = JsonConvert.SerializeObject(response);
            return Page();
        }
    }
}