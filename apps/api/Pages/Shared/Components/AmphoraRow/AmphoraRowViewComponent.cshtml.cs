using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "AmphoraRow")]
    public class AmphoraRowViewComponent : ViewComponent
    {
        public AmphoraModel Amphora { get; private set; }
        public bool IsSelectable { get; private set; }
        public int Index { get; private set; }

        public IViewComponentResult Invoke(AmphoraModel amphora, bool isTable, int index, bool isSelectable = false)
        {
            this.Amphora = amphora;
            this.IsSelectable = isSelectable;
            this.Index = index;
            return View(this);
        }
    }
}