using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Schemas.Library;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Temporae
{
    [ValidateAntiForgeryToken]
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly IEntityStore<Schema> schemaStore;
        private readonly IMapper mapper;

        public CreateModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IEntityStore<Schema> schemaStore,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.schemaStore = schemaStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Tempora Tempora { get; set; }
        public IEnumerable<Schema> Schemas { get; set; }

        private async Task LoadAvailableSchemas()
        {
            var stored = await schemaStore.ListAsync();
            var library = new SchemaLibrary();
            var libraryCollection = library.List();
            libraryCollection.AddRange(stored);
            this.Schemas = libraryCollection;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAvailableSchemas();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var entity = mapper.Map<Amphora.Common.Models.Tempora>(Tempora);
                var setResult = await temporaEntityStore.CreateAsync(entity);
                return RedirectToPage("/Temporae/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}