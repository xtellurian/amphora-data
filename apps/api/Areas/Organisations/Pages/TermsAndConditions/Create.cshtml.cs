using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.TermsAndConditions
{
    [CommonAuthorize]
    public class CreateModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly ITermsOfUseService termsOfUseService;
        private readonly IMapper mapper;

        public CreateModel(IOrganisationService organisationService, ITermsOfUseService termsOfUseService, IMapper mapper)
        {
            this.organisationService = organisationService;
            this.termsOfUseService = termsOfUseService;
            this.mapper = mapper;
        }

        [BindProperty]
        public Amphora.Api.Models.Dtos.Organisations.TermsOfUse TermsOfUse { get; set; }
        public OrganisationModel Organisation { get; private set; }
        [TempData]
        public bool? Success { get; set; } = null;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var result = await organisationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                return Page();
            }
            else
            {
                return RedirectToPage("../Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var result = await organisationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var model = mapper.Map<TermsOfUseModel>(this.TermsOfUse);
                var createResult = await termsOfUseService.CreateAsync(User, model);
                if (createResult.Failed)
                {
                    // duplicate id
                    ModelState.AddModelError(string.Empty, createResult.Message);
                    Success = false;
                    return Page();
                }

                var updateResult = await organisationService.UpdateAsync(User, Organisation);
                if (updateResult.Succeeded)
                {
                    Success = true;
                    return RedirectToPage("./Index", new { Id = Organisation.Id });
                }
                else
                {
                    Success = false;
                    this.ModelState.AddModelError(string.Empty, updateResult.Message);
                }

                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return RedirectToPage("../Index");
            }
        }
    }
}