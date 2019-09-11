using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;

        [TempData]
        public string ErrorMessage { get; set; }
        public EditModel(
            IEntityStore<OrganisationModel> orgStore,
            IUserService userService,
            IPermissionService permissionService)
        {
            this.orgStore = orgStore;
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
            var organisation = await orgStore.ReadAsync<OrganisationExtendedModel>(id, id);

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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (ModelState.IsValid)
            {
                var user = await userService.UserManager.GetUserAsync(User);
                if (id == null) id = user.OrganisationId;
                var organisation = await orgStore.ReadAsync<OrganisationExtendedModel>(id, id);

                var authorized = await permissionService.IsAuthorizedAsync(user, organisation, ResourcePermissions.Update);
                if(authorized)
                {
                    organisation.Name = Input.Name;
                    organisation.About = Input.About;
                    await this.orgStore.UpdateAsync(organisation);
                }
                return RedirectToPage("./Detail", new {Id = organisation.OrganisationId});
            }
            else
            {
                ErrorMessage = "Invalid User Details";
                return Page();
            }
        }
    }
}