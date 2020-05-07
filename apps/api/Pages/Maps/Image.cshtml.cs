using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Maps
{
    public class ImagePageModel : PageModel
    {
        private readonly IMapService mapService;

        public ImagePageModel(IMapService mapService)
        {
            this.mapService = mapService;
        }

        public async Task<IActionResult> OnGetAsync(double? lat, double? lon, int? height, int? width)
        {
            if (lat.HasValue && lon.HasValue)
            {
                var geo = new GeoLocation(lon.Value, lat.Value);
                byte[] image;
                if (height.HasValue && width.HasValue)
                {
                    image = await mapService.GetStaticMapImageAsync(geo, height.Value, width.Value);
                }
                else
                {
                    image = await mapService.GetStaticMapImageAsync(geo);
                }

                if (image == null) { return Redirect("/images/stock/earth.jpg"); }
                return File(image, "image/png"); // image is a png
            }
            else
            {
                return Redirect("/images/stock/earth.jpg");
            }
        }
    }
}