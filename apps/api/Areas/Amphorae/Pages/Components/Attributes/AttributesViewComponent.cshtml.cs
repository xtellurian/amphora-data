using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "Attributes")]
    public class AttributesViewComponent : ViewComponent
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public IDictionary<string, string> Attributes { get; private set; }

        public IViewComponentResult Invoke(string id, string title, IDictionary<string, string> attributes)
        {
            this.Id = id;
            this.Title = title ?? "Attributes";
            this.Attributes = attributes;
            return View(this);
        }
    }
}