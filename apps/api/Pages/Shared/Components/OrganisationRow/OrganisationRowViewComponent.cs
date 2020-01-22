using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "OrganisationRow")]
    public class OrganisationRowViewComponent : ViewComponent
    {
        public OrganisationModel Org { get; private set; }

        public IViewComponentResult Invoke(OrganisationModel org, bool isTable)
        {
            this.Org = org;
            if (isTable) { return View("Table", this); }
            else { return View(this); }
        }
    }
}