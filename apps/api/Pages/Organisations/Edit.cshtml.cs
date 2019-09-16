using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;

        [TempData]
        public string ErrorMessage { get; set; }
        public EditModel(
            IOrganisationService organisationService,
            IUserService userService,
            IPermissionService permissionService)
        {
            this.organisationService = organisationService;
            this.userService = userService;
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
            var user = await userService.UserManager.GetUserAsync(User);
            if (id == null) id = user.OrganisationId;
            this.OrganisationId = id;
            var organisation = await organisationService.Store.ReadAsync<OrganisationExtendedModel>(id, id);

            var authorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Update);
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
                var user = await userService.UserManager.GetUserAsync(User);
                if (id == null) id = user.OrganisationId;
                var organisation = await organisationService.Store.ReadAsync<OrganisationExtendedModel>(id, id);

                var authorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Update);
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
                return RedirectToPage("./Detail", new { Id = organisation.OrganisationId });
            }
            else
            {
                ErrorMessage = "Invalid User Details";
                return Page();
            }
        }
    }
}