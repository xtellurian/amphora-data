using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Components
{
    [ViewComponent(Name = "Metadata")]
    public class MetadataViewComponent : ViewComponent
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public MetaDataStore Meta { get; private set; }

        public IViewComponentResult Invoke(string id, string title, MetaDataStore meta)
        {
            this.Id = id;
            this.Title = title ?? "Metadata";
            this.Meta = meta;
            return View(this);
        }
    }
}