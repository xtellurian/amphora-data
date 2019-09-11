using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IAuthenticateService authenticateService;
        private readonly ISignInManager signInManager;
        private readonly ILogger<CreateModel> logger;

        public CreateModel(IOrganisationService organisationService,
                           IAuthenticateService authenticateService,
                           ISignInManager signInManager,
                           ILogger<CreateModel> logger)
        {
            this.organisationService = organisationService;
            this.authenticateService = authenticateService;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public string Token { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [DataType(DataType.Text)]
            public string Name { get; set; }
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }
            [DataType(DataType.Url)]
            public string Website { get; set; }
            [DataType(DataType.Text)]
            public string Address { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await authenticateService.GetToken(User);
            if (response.success)
            {
                Token = response.token;
            }
            else
            {
                logger.LogError("Couldn't get token");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var org = new OrganisationModel
            {
                Name = Input.Name,
                Description = Input.Description,
                WebsiteUrl = Input.Website,
                Address = Input.Address,
            };

            var result = await organisationService.CreateOrganisationAsync(User, org);
            if (result.Succeeded)
            {
                return RedirectToPage("/Organisations/Detail");
            }
            else
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }
                return Page();
            }
        }
    }
}