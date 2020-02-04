using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [Authorize]
    public class EditMetadataPageModel : AmphoraPageModel
    {
        public EditMetadataPageModel(IAmphoraeService amphoraeService) : base(amphoraeService)
        {
        }

        [BindProperty]
        public MetaDataStore Metadata { get; private set; } = new MetaDataStore();

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            await LoadAmphoraAsync(id);
            if (Amphora != null)
            {
                if (Amphora.FilesMetaData.TryGetValue(name, out var meta))
                {
                    this.Metadata = meta;
                }
            }

            return Page();
        }
    }
}