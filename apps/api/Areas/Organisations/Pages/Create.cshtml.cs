using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages
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
        public IList<OrganisationModel> Organisations { get; set; }

        public class InputModel
        {
            [DataType(DataType.Text)]
            public string Name { get; set; }
            [DataType(DataType.MultilineText)]
            public string About { get; set; }
            [DataType(DataType.Url)]
            public string Website { get; set; }
            [DataType(DataType.Text)]
            public string Address { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string message)
        {
            if (!string.IsNullOrEmpty(message)) { this.ModelState.AddModelError(string.Empty, message); }
            var response = await authenticateService.GetToken(User);
            Organisations = await organisationService.Store.TopAsync();
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

        public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
        {
            var org = new OrganisationModel(Input.Name, Input.About, Input.Website, Input.Address);

            var result = await organisationService.CreateOrganisationAsync(User, org);
            if (result.Succeeded)
            {
                var formFile = files.FirstOrDefault();

                if (formFile != null && formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        await this.organisationService.WriteProfilePictureJpg(result.Entity, await stream.ReadFullyAsync());
                    }
                }

                return RedirectToPage("/Home/Index");
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