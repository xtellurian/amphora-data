using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Shared
{
    public class UnauthorizedModel : PageModel
    {
        public string ResourceId { get; set; }

        public void OnGet(string resourceId)
        {
            ResourceId = resourceId;
        }

    }
}