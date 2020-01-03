using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "AmphoraRow")]
    public class AmphoraRowViewComponent : ViewComponent
    {
        public AmphoraModel Amphora { get; private set; }

        public IViewComponentResult Invoke(AmphoraModel amphora, bool isTable)
        {
            this.Amphora = amphora;
            if (isTable) return View("Table", this);
            else return View(this);
        }
    }
}