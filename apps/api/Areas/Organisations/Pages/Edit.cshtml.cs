using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class EditModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;

        public EditModel(
            IOrganisationService organisationService,
            IUserDataService userDataService,
            IPermissionService permissionService)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
        }

        public class InputModel
        {
            [DataType(DataType.Text)]
            public string Name { get; set; }
            [DataType(DataType.Text)]
            public string About { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string OrganisationId { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var userReadRes = await userDataService.ReadAsync(User);
            this.OrganisationId = id;

            if (!userReadRes.Succeeded)
            {
                ModelState.AddModelError(string.Empty, userReadRes.Message);
                return Page();
            }

            var userData = userReadRes.Entity;
            if (id == null) { id = userData.OrganisationId; }
            var organisation = await organisationService.Store.ReadAsync(id);

            var authorized = await permissionService.IsAuthorizedAsync(userData, organisation, ResourcePermissions.Update);
            if (authorized)
            {
                this.Input = new InputModel
                {
                    Name = organisation.Name,
                    About = organisation.About
                };
                return Page();
            }
            else
            {
                return RedirectToPage("Shared/Unauthorized", new { resourceId = organisation.Id });
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var userReadRes = await userDataService.ReadAsync(User);
                if (!userReadRes.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, userReadRes.Message);
                    return Page();
                }

                var userData = userReadRes.Entity;
                if (id == null) { id = userData.OrganisationId; }
                var organisation = await organisationService.Store.ReadAsync(id);

                var authorized = await permissionService.IsAuthorizedAsync(userData, organisation, ResourcePermissions.Update);
                if (authorized)
                {
                    organisation.Name = Input.Name;
                    organisation.About = Input.About;
                    await this.organisationService.Store.UpdateAsync(organisation);

                    var formFile = files.FirstOrDefault();

                    if (formFile != null && formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            await this.organisationService.WriteProfilePictureJpg(organisation, await stream.ReadFullyAsync());
                        }
                    }
                }

                return RedirectToPage("./Detail", new { Id = organisation.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid User Model");
                return Page();
            }
        }
    }
}