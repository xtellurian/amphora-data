using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class CreateModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IInvitationService invitationService;
        private readonly ILogger<CreateModel> logger;

        public CreateModel(IOrganisationService organisationService,
                           IInvitationService invitationService,
                           ILogger<CreateModel> logger)
        {
            this.organisationService = organisationService;
            this.invitationService = invitationService;
            this.logger = logger;
        }

        [TempData]
        public string CreateOrganisationMessage { get; set; }

        [BindProperty]
        public Organisation Input { get; set; }
        public IList<OrganisationModel> Organisations { get; set; }
        public InvitationModel Invitation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.CreateOrganisationMessage = message;
            }

            var res = await invitationService.GetMyInvitations(User);
            if (res.Succeeded)
            {
                this.Invitation = res.Entity.FirstOrDefault();
            }

            Organisations = await organisationService.Store.TopAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var org = new OrganisationModel(Input.Name, Input.About, Input.WebsiteUrl, Input.Address);

                var result = await organisationService.CreateAsync(User, org);
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

                    // Happy Path
                    return RedirectToPage("/Plans/SelectPlan");
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
            else
            {
                return Page();
            }
        }
    }
}