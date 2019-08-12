using System.Threading.Tasks;
using Amphora.Api.Contracts;
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
        private readonly IMapper mapper;

        public CreateModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Tempora Tempora { get; set; }

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