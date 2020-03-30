using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "AmphoraQualityBadges")]
    public class AmphoraQualityBadgesViewComponent : ViewComponent
    {
        public DataQualitySummary Quality { get; private set; }

        public IViewComponentResult Invoke(DataQualitySummary qualitySummary)
        {
            this.Quality = qualitySummary;
            return View(this);
        }
    }
}