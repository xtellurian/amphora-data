using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class FileModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public FileModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IPermissionService permissionService,
            IUserService userService,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.blobStore = blobStore;
            this.permissionService = permissionService;
            this.userService = userService;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            if(string.IsNullOrEmpty(name)) return RedirectToPage("./Detail", new {Id = id});
            var entity = await amphoraeService.AmphoraStore.ReadAsync(id);
            if (entity == null)
            {
                return RedirectToPage("/amphorae/index");
            }
            var user = await userService.UserManager.GetUserAsync(User);
            if (await permissionService.IsAuthorizedAsync(user, entity, Common.Models.Permissions.AccessLevels.ReadContents))
            {
                var file = await blobStore.ReadBytesAsync(entity, name);
                if(file == null || file.Length == 0) return RedirectToPage("./Detail", new {Id = id});
                return File(file, "application/octet-stream", name);
            }
            else
            {
                return RedirectToPage("./Forbidden");
            }
        }
    }
}