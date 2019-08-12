using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Pages.Temporae
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly IDataStore<Common.Models.Tempora, Datum> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public DetailModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IDataStore<Amphora.Common.Models.Tempora, Datum> dataStore,
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
        public Amphora.Common.Models.Domains.Domain Domain { get; set; }
        public string Token { get; set; } = "blank for now";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Tempora = await temporaEntityStore.ReadAsync(id);
            if (Tempora == null)
            {
                return RedirectToPage("/amphorae/index");
            }

            LoadDomain(Tempora.DomainId);

            return Page();
        }

        private void LoadDomain(string domainId)
        {
            Domain = Common.Models.Domains.Domain.GetDomain(domainId);
        }

    }
}