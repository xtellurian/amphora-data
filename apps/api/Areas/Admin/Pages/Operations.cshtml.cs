using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages
{
    [GlobalAdminAuthorize]
    public class OperationsPageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;

        public OperationsPageModel(IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        [TempData]
        public string CompletedMessage { get; set; }

        public async Task<IActionResult> OnPostSignalsV2MigrateAsync()
        {
            var allAmphora = amphoraeService.AmphoraStore.Query(_ => true);
            foreach (var a in allAmphora)
            {
                a.EnsureV2Signals();
                await amphoraeService.AmphoraStore.UpdateAsync(a);
            }
            this.CompletedMessage = "Finish Ensuring V2 Signals on all Amphora";
            return Page();
        }
    }


}