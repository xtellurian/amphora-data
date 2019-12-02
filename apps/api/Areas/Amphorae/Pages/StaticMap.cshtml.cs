using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> OnGetAsync(string id, int? height, int? width)
        {
            await base.LoadAmphoraAsync(id);
            if (Amphora != null && Amphora.GeoLocation != null && Amphora.GeoLocation.Lat().HasValue && Amphora.GeoLocation.Lon().HasValue)
            {
                byte[] image;
                if (height.HasValue && width.HasValue)
                {
                    image = await mapService.GetStaticMapImageAsync(Amphora.GeoLocation, height.Value, width.Value);
                }
                else
                {
                    image = await mapService.GetStaticMapImageAsync(Amphora.GeoLocation);
                }
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