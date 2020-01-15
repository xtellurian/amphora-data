using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "Map")]
    public class MapViewComponent : ViewComponent
    {
        public GeoLocation Geo { get; private set; }

        public IViewComponentResult Invoke(GeoLocation geo)
        {
            this.Geo = geo;
            return View(this);
        }
    }
}