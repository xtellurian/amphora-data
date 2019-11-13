using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.TermsAndConditions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public CreateModel(IOrganisationService organisationService, IMapper mapper)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        [BindProperty]
        public TermsAndConditionsDto TermsAndConditions { get; set; }
        public OrganisationModel Organisation { get; private set; }

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
                if(!ModelState.IsValid)
                {
                    return Page();
                }
                var model = mapper.Map<TermsAndConditionsModel>(this.TermsAndConditions);
                if(! this.Organisation.AddTermsAndConditions(model))
                {
                    // duplicate id
                    ModelState.AddModelError(string.Empty, $"The Id '{model.Id}' already exists.");
                    return Page();
                }
                
                var updateResult = await organisationService.UpdateAsync(User, Organisation);
                if (updateResult.Succeeded) return RedirectToPage("./Index", new { Id = Organisation.Id });
                else this.ModelState.AddModelError(string.Empty, updateResult.Message);
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