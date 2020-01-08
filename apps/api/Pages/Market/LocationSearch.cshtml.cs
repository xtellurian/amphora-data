using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Market
{
    [Authorize]
    public class LocationSearchModel : PageModel
    {
        private readonly IMapService mapService;

        public LocationSearchModel(IMapService mapService)
        {
            this.mapService = mapService;
        }

        public async Task<IActionResult> OnGetAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            return new JsonResult(response);
        }
    }
}