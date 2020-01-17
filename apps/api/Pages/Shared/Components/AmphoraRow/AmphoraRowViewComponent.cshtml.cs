using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "AmphoraRow")]
    public class AmphoraRowViewComponent : ViewComponent
    {
        public AmphoraModel Amphora { get; private set; }
        public bool IsSelectable { get; private set; }

        public IViewComponentResult Invoke(AmphoraModel amphora, bool isTable, bool isSelectable = false)
        {
            this.Amphora = amphora;
            this.IsSelectable = isSelectable;
            if (isTable) { return View("Table", this); }
            else { return View(this); }
        }
    }
}