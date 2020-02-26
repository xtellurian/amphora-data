using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "RestrictionRow")]
    public class RestrictionRowViewComponent : ViewComponent
    {
        public RestrictionModel Restriction { get; private set; }
        public int Index { get; private set; }

        public IViewComponentResult Invoke(RestrictionModel restriction, int index = -1)
        {
            this.Restriction = restriction;
            this.Index = index;
            return View(this);
        }
    }
}