using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Amphora.Api.Models;
using Amphora.Api.Contracts;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using Amphora.Common.Models;

namespace Amphora.Api.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ISignInManager signInManager;
        private readonly IUserManager userManager;
        private readonly IOnboardingService onboardingService;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEntityStore<Organisation> orgStore;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            IUserManager userManager,
            IUserService userService,
            IOnboardingService onboardingService,
            ISignInManager signInManager,
            IOptionsMonitor<RegistrationOptions> registrationOptions,
            ILogger<RegisterModel> logger,
            IEntityStore<Organisation> orgStore,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.onboardingService = onboardingService;
            this.signInManager = signInManager;
            this.logger = logger;
            this.orgStore = orgStore;
            this.emailSender = emailSender;

        }

        [BindProperty]
        public InputModel Input { get; set; }
        public Organisation Organisation { get; private set; }
        public string ReturnUrl { get; set; }
        public string OnboardingId { get; private set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [DataType(DataType.MultilineText)]
            [Display(Name = "About")]
            public string About { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        public async Task<IActionResult> OnGetAsync(string onboardingId, string returnUrl = null)
        {
            this.OnboardingId = onboardingId;
            if(! string.IsNullOrEmpty(onboardingId))
            {
                this.Organisation = await onboardingService.GetOrganisationFromOnboardingId(onboardingId);
            }
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string onboardingId, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    About = Input.About,
                    FullName = Input.FullName,
                    IsOnboarding = true
                };

                var result = await onboardingService.CreateUserAsync(user, Input.Password, onboardingId);
                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");
                    await SendConfirmationEmailAsync(result.Entity);
                    if (string.IsNullOrEmpty(result.Entity.OrganisationId))
                    {
                        return RedirectToPage("/Organisations/Create", new { OnboardingId = result.Entity.OnboardingId });
                    }
                    return RedirectToPage(returnUrl);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SendConfirmationEmailAsync(IApplicationUser user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user); // bug here
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            await signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
