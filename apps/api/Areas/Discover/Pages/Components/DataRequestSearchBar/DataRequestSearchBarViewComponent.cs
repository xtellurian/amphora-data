using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Areas.Discover.Pages.Components
{
    [ViewComponent(Name = "DataRequestSearchBar")]
    public class DataRequestSearchBarViewComponent : SearchBarViewModel
    {
        [BindProperty]
        public DataRequestSearchQueryOptions SearchDefinition { get; private set; }
        public IList<FacetResult> LabelFacets { get; private set; } = new List<FacetResult>();
        public IViewComponentResult Invoke(DataRequestSearchQueryOptions searchDefinition,
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