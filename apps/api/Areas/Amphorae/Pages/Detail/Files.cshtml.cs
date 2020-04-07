using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class FilesPageModel : AmphoraDetailPageModel
    {
        private readonly IAmphoraFileService amphoraFileService;

        public FilesPageModel(IAmphoraeService amphoraeService,
                              IQualityEstimatorService qualityEstimator,
                              IPurchaseService purchaseService,
                              IPermissionService permissionService,
                              IAmphoraFileService amphoraFileService) : base(amphoraeService, qualityEstimator, purchaseService, permissionService)
        {
            this.amphoraFileService = amphoraFileService;
        }

        public IList<string> Names { get; private set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            TryLoadPurchase();
            await LoadNames();
            if (CanReadContents)
            {
                return OnReturnPage();
            }
            else
            {
                return RedirectToPage("/Detail/Index", new { id = id });
            }
        }

        public async Task<IActionResult> OnPostUploadAsync(string id, List<IFormFile> files)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            TryLoadPurchase();
            await LoadNames();

            if (Amphora == null)
            {
                return BadRequest(Error);
            }

            if (files == null || files.Count > 1)
            {
                ModelState.AddModelError(string.Empty, "You can only upload 1 file at a time");
                return Page();
            }

            if (CanWriteContents)
            {
                var formFile = files.FirstOrDefault();

                if (formFile != null && formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        var res = await this.amphoraFileService.WriteFileAsync(User, Amphora, await stream.ReadFullyAsync(), formFile.FileName);

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
                return Forbid();
            }

            await LoadNames();
            return Page();
        }

        private async Task LoadNames()
        {
            if (Amphora != null)
            {
                Names = await amphoraFileService.Store.ListBlobsAsync(Amphora);
            }
        }
    }
}