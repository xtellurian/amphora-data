using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Home
{
    public class ErrorModel: PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}