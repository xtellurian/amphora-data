using System;
using Amphora.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Pages.Redirection
{
    public class FeedbackModel : PageModel
    {
        private readonly IOptionsMonitor<FeedbackOptions> options;
        private readonly ILogger<FeedbackModel> logger;
        private string BodyTemplate(string from) => $">Please Keep this section. Issue from '{from}'.%0A";

        public FeedbackModel(IOptionsMonitor<FeedbackOptions> options, ILogger<FeedbackModel> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public IActionResult OnGet(string from)
        {
            if (options.CurrentValue.RedirectUrl == null) { throw new NullReferenceException("Feedback URL was null"); }
            var red = $"{options.CurrentValue.RedirectUrl}?body={BodyTemplate(from)}";
            logger.LogInformation($"Redirecting user to {red}");
            return Redirect(red);
        }
    }
}