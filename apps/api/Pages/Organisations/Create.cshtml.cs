using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IEntityStore<Organisation> entityStore;
        private readonly IAuthenticateService authenticateService;
        private readonly ILogger<CreateModel> logger;

        public CreateModel(IEntityStore<Organisation> entityStore,
                           IAuthenticateService authenticateService,
                           ILogger<CreateModel> logger)
        {
            this.entityStore = entityStore;
            this.authenticateService = authenticateService;
            this.logger = logger;
        }

        public string Token {get; set; }

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
            if(response.success)
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
            var org = new Organisation
            {
                Name = Input.Name,
                Description = Input.Description,
                WebsiteUrl = Input.Website,
                Address = Input.Address,
            };
            org = await entityStore.CreateAsync(org);
            return RedirectToPage("./Detail", new { Id = org.OrganisationId });
        }
    }
}