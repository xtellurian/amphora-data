using System.Collections.Generic;
using Amphora.Api.Models.Dtos.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;

namespace Amphora.Api.Areas.Discover.Pages.Components
{
    [ViewComponent(Name = "AmphoraSearchBar")]
    public class AmphoraSearchBarViewComponent : SearchBarViewModel
    {
        [BindProperty]
        public AmphoraSearchQueryOptions Q { get; private set; }
        public IList<FacetResult> LabelFacets { get; private set; } = new List<FacetResult>();
        public IViewComponentResult Invoke(AmphoraSearchQueryOptions q,
                                           IList<FacetResult> labelfacets,
                                           string formId,
                                           bool isAdvanced,
                                           string handler = null,
                                           string placeholder = null)
        {
            LabelFacets = labelfacets;
            Q = q;
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