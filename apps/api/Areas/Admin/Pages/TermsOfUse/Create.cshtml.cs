using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Terms;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.TermsOfUse
{
    [GlobalAdminAuthorize]
    public class CreatePageModel : PageModel
    {
        private readonly ITermsOfUseService termsOfUseService;
        private readonly IMapper mapper;

        public CreatePageModel(ITermsOfUseService termsOfUseService, IMapper mapper)
        {
            this.termsOfUseService = termsOfUseService;
            this.mapper = mapper;
        }

        [BindProperty]
        public CreateTermsOfUse NewTerms { get; set; } = new CreateTermsOfUse();

        public async Task<IActionResult> OnPostAsync()
        {
            var model = mapper.Map<TermsOfUseModel>(this.NewTerms);

            var createRes = await termsOfUseService.CreateGlobalAsync(User, model);
            if (createRes.Succeeded)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, createRes.Message);
                return Page();
            }
        }
    }
}