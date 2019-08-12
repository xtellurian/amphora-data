using System.Threading.Tasks;
using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    [ValidateAntiForgeryToken]
    public class CreateModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IMapper mapper;

        public CreateModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Amphora Amphora { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var entity = mapper.Map<Amphora.Common.Models.Amphora>(Amphora);
                var setResult = await amphoraEntityStore.CreateAsync(entity);
                return RedirectToPage("/Amphorae/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}