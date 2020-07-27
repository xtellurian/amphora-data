using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Discover.Pages.Components
{
    public abstract class SearchBarViewModel : ViewComponent
    {
        public string FormId { get; protected set; }
        public string Handler { get; protected set; }
        public string Placeholder { get; protected set; }
    }
}