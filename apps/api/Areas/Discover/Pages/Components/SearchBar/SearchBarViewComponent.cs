using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Areas.Discover.Pages.Components
{
    [ViewComponent(Name = "SearchBar")]
    public class SearchBarViewComponent : ViewComponent
    {
        public string FormId { get; private set; }
        public string Handler { get; private set; }
        public string Placeholder { get; private set; }
        [BindProperty]
        public MarketSearch SearchDefinition { get; private set; }
        public IList<FacetResult> LabelFacets { get; private set; } = new List<FacetResult>();
        public IViewComponentResult Invoke(MarketSearch searchDefinition,
                                           IList<FacetResult> labelfacets,
                                           string formId,
                                           bool isAdvanced,
                                           string handler = null,
                                           string placeholder = null)
        {
            LabelFacets = labelfacets;
            SearchDefinition = searchDefinition;
            FormId = formId;
            this.Handler = handler ?? "";
            Placeholder = placeholder ?? "What are you searching for?";
            if (isAdvanced)
            {
                return View("Advanced", this);
            }

            return View(this);
        }
    }
}