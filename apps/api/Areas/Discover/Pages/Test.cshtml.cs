using Amphora.Api.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Discover.Pages
{
    public class TestPageModel : PageModel
    {
        public TestPageModel(IOptionsMonitor<AzureMapsOptions> options)
        {
            this.Key = options.CurrentValue.Key;
        }

        public string Key { get; }
    }
}