using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "FilePreview")]
    public class FilePreviewViewComponent : ViewComponent
    {
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public FilePreviewViewComponent(IPermissionService permissionService, IUserService userService)
        {
            this.permissionService = permissionService;
            this.userService = userService;
        }

        public string Id { get; private set; }
        public AmphoraModel Amphora { get; private set; }
        public string Name { get; private set; }
        public bool IsAuthorized { get; private set; }
        public string RelPath { get; private set; }

        public async Task<IViewComponentResult> InvokeAsync(string id, AmphoraModel amphora, string name, ApplicationUser user)
        {
            this.Id = id;
            this.Amphora = amphora;
            this.Name = name;
            this.IsAuthorized = await permissionService.IsAuthorizedAsync(user, amphora, AccessLevels.ReadContents);
            this.RelPath = $"Files/Download?id={Amphora.Id}&name={Name}";
            return View(this);
        }
    }
}