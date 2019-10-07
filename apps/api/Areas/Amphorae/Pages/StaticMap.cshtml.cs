using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class StaticMapModel : AmphoraPageModel
    {
        private readonly IMapService mapService;

        public StaticMapModel(IAmphoraeService amphoraeService, IMapService mapService) : base(amphoraeService)
        {
            this.mapService = mapService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            if (Amphora != null && Amphora.GeoLocation != null && Amphora.GeoLocation.Lat().HasValue && Amphora.GeoLocation.Lon().HasValue)
            {
                var image = await mapService.GetStaticMapImageAsync(Amphora.GeoLocation);
                if (image == null) return Redirect("/images/stock/earth.jpg");
                return File(image, "image/png"); // image is a png
            }
            else
            {
                return Redirect("/images/stock/earth.jpg");
            }
        }
    }
}