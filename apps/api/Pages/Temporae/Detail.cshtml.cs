using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Schemas.Library;
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
        private readonly IDataStore<Common.Models.Tempora, JObject> dataStore;
        private readonly ITsiService tsiService;
        private readonly IEntityStore<Schema> schemaStore;
        private readonly IMapper mapper;

        public DetailModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IDataStore<Amphora.Common.Models.Tempora, JObject> dataStore,
            ITsiService tsiService,
            IEntityStore<Schema> schemaStore,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.schemaStore = schemaStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Tempora Tempora { get; set; }
        public Amphora.Common.Models.Schema Schema { get; set; }
        public string Token { get; set; } = "blank for now";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Tempora = await temporaEntityStore.ReadAsync(id);
            await LoadAvailableSchemas();
            if (Tempora == null)
            {
                return RedirectToPage("/amphorae/index");
            }

            return Page();
        }

         private async Task LoadAvailableSchemas()
        {
            var library = new SchemaLibrary();
            var schema = library.Load(Tempora.SchemaId);
            if(schema == null)
            {
                schema = await schemaStore.ReadAsync(Tempora.SchemaId);
            }

            this.Schema = schema;
        }

    }
}