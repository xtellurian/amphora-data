using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [DisableRequestSizeLimit]
    public class IndexPageModel : AmphoraPageModel
    {
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly IAmphoraFileService amphoraFileService;

        public IndexPageModel(IAmphoraeService amphoraeService,
                              IPermissionService permissionService,
                              IUserService userService,
                              IAmphoraFileService amphoraFileService) : base(amphoraeService)
        {
            this.permissionService = permissionService;
            this.userService = userService;
            this.amphoraFileService = amphoraFileService;
        }

        public IList<string> Names { get; private set; }
        public bool CanDeleteFiles { get; private set; }
        public bool CanUploadFiles { get; private set; }
        public bool CanDownloadFiles { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostUploadAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) { return RedirectToPage("../Index"); }

            var result = await amphoraeService.ReadAsync(User, id);
            var user = result.User;

            if (result.Succeeded)
            {
                if (result.Entity == null) { return RedirectToPage("../Index"); }

                if (await permissionService.IsAuthorizedAsync(user, result.Entity, AccessLevels.WriteContents))
                {
                    var formFile = files.FirstOrDefault();

                    if (formFile != null && formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var res = await this.amphoraFileService.WriteFileAsync(User, result.Entity, await stream.ReadFullyAsync(), formFile.FileName);

                            if (!res.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, res.Message);
                                return Page();
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToPage("./Forbidden");
                }

                this.Amphora = result.Entity;
                await SetPagePropertiesAsync();
                return RedirectToPage("./Index", new { Id = id });
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("../Forbidden");
            }
            else
            {
                return RedirectToPage("../Index");
            }
        }

        private async Task SetPagePropertiesAsync()
        {
            if (Amphora != null)
            {
                var user = await userService.ReadUserModelAsync(User);
                Names = await amphoraFileService.Store.ListBlobsAsync(Amphora);
                CanDeleteFiles = await permissionService.IsAuthorizedAsync(user, Amphora, AccessLevels.Update);
                // set the three types of permission for the page
                if (CanDeleteFiles)
                {
                    CanUploadFiles = true;
                }
                else
                {
                    CanUploadFiles = await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.WriteContents);
                }

                if (CanUploadFiles)
                {
                    CanDownloadFiles = true;
                }
                else
                {
                    CanDownloadFiles = await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.ReadContents);
                }
            }
        }
    }
}