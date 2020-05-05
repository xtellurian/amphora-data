using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "AmphoraQualityBadges")]
    public class AmphoraQualityBadgesViewComponent : ViewComponent
    {
        public EnrichedDataQuality Quality { get; private set; }

        public IViewComponentResult Invoke(EnrichedDataQuality qualitySummary)
        {
            this.Quality = qualitySummary;
            return View(this);
        }
    }
}