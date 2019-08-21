using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Domains;
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
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> temporaEntityStore;
        private readonly IMapper mapper;

        public CreateModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> temporaEntityStore,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Common.Models.Amphora Tempora { get; set; }
        public IEnumerable<Domain> Domains { get; set; }

        public IActionResult OnGet()
        {
            Domains = Domain.GetAllDomains();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var entity = mapper.Map<Amphora.Common.Models.Amphora>(Tempora);
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