using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "AmphoraTableRow")]
    public class AmphoraTableRowViewComponent: ViewComponent
    {
        public AmphoraModel Amphora { get; private set; }

        public IViewComponentResult Invoke(AmphoraModel amphora)
        {
            this.Amphora = amphora;
            return View("AmphoraTableRowViewComponent", this);
        }
    }
}