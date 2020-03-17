using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.DataAnnotations;
using Amphora.Common.Models.Host;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModelBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOptionsMonitor<HostOptions> hostOptions;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IOptionsMonitor<HostOptions> hostOptions,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.hostOptions = hostOptions;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        [BindProperty]
        [IsTrue(ErrorMessage = "You must accept the service agreement.")]
        public bool AcceptServiceAgreement { get; set; }
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full Name")]
            public string? FullName { get; set; }

            [DataType(DataType.MultilineText)]
            [Display(Name = "About")]
            public string? About { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        public IActionResult OnGet(string? returnUrl = null, string? email = null)
        {
            ReturnUrl = returnUrl;

            if (email != null)
            {
                Input.Email = email;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    About = Input.About,
                    FullName = Input.FullName
                };

                // TODO invitation
                // var invitation = await invitationService.GetInvitationByEmailAsync(Input.Email);

                var result = await userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");
                    await SendConfirmationEmailAsync(user);

                    await signInManager.SignInAsync(user, isPersistent: false);

                    // if (string.IsNullOrEmpty(invitation?.TargetOrganisationId))
                    // {
                    return this.LoadingPage("/Redirect", this.ReturnUrl);
                    // }
                    // else
                    // {
                    //     return RedirectToPage("/Join", new { area = "organisations", orgId = invitation?.TargetOrganisationId });
                    // }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            // the links are broken when you do it this way
            // if (!string.IsNullOrEmpty(hostOptions.CurrentValue?.MainHost))
            // {
            //     await emailSender.SendEmailAsync(new ConfirmEmailEmail(user, hostOptions.CurrentValue, code));
            // }
            // else
            // {
            //     await emailSender.SendEmailAsync(Input.Email, "Please confirm your email",
            //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            // }

            await emailSender.SendEmailAsync(Input.Email!, "Please confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
