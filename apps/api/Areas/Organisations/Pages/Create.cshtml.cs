using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
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
        private readonly IInvitationService invitationService;
        private readonly ISignInManager signInManager;
        private readonly ILogger<CreateModel> logger;

        public CreateModel(IOrganisationService organisationService,
                           IAuthenticateService authenticateService,
                           IInvitationService invitationService,
                           ISignInManager signInManager,
                           ILogger<CreateModel> logger)
        {
            this.organisationService = organisationService;
            this.authenticateService = authenticateService;
            this.invitationService = invitationService;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public string Token { get; set; }

        [BindProperty]
        public Organisation Input { get; set; }
        public IList<OrganisationModel> Organisations { get; set; }
        public InvitationModel Invitation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string message)
        {
            if (!string.IsNullOrEmpty(message)) { this.ModelState.AddModelError(string.Empty, message); }
            var res = await invitationService.GetMyInvitations(User);
            if (res.Succeeded)
            {
                this.Invitation = res.Entity.FirstOrDefault();
            }

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
            else
            {
                return Page();
            }
        }
    }
}