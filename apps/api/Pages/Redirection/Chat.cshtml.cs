using Amphora.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Pages.Redirection
{
    public class ChatModel: PageModel
    {
        private readonly IOptionsMonitor<ChatOptions> options;
        private readonly ILogger<ChatModel> logger;

        public ChatModel(IOptionsMonitor<ChatOptions> options, ILogger<ChatModel> logger)
        {
            this.options = options;
            this.logger = logger;
        }
        public IActionResult OnGet()
        {
            var red = options.CurrentValue.RedirectUrl;
            logger.LogInformation($"Redirecting user to {red}");
            return Redirect(red);
        }
    }
}