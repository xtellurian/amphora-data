using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Pages.Temporae
{
    public class DetailModel : PageModel
    {
        private readonly IOrgEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly IDataStore<Common.Models.Tempora, JObject> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public DetailModel(
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IDataStore<Amphora.Common.Models.Tempora, JObject> dataStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Tempora Tempora { get; set; }
        public string Token { get; set; } = "blank for now";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Tempora = await temporaEntityStore.ReadAsync(id);
            if (Tempora == null)
            {
                return RedirectToPage("/amphorae/index");
            }

            return Page();
        }

    }
}