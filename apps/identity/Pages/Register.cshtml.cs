using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.DataAnnotations;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Options;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Pages
{
    public class RegisterPageModel : RegisterPageModelBase
    {
        private readonly IOptionsMonitor<ExternalServices> externalSvcsOptions;

        public RegisterPageModel(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<RegisterPageModel> logger,
                                 IOptionsMonitor<ExternalServices> externalSvcsOptions,
                                 IEmailSender emailSender) : base(userManager, signInManager, logger, emailSender)
        {
            this.externalSvcsOptions = externalSvcsOptions;
        }

        [BindProperty]
        [IsTrue(ErrorMessage = "You must accept the service agreement.")]
        public bool AcceptServiceAgreement { get; set; }
        [BindProperty]
        public RegisterUser Registration { get; set; } = new RegisterUser();

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Registration.Username,
                    Email = Registration.Email
                };

                returnUrl ??= externalSvcsOptions.CurrentValue?.WebAppUri()?.ToStandardString() + "/Challenge";
                return await HandleRegistration(returnUrl, user, Registration.Password!);
            }
            else
            {
                return Page();
            }
        }
    }
}