using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Emails;
using Amphora.Api.Models.Host;
using Amphora.Api.Options;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IOptionsMonitor<HostOptions> hostOptions;
        private readonly ISignInManager signInManager;
        private readonly IInvitationService invitationService;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            IUserManager userManager,
            IUserService userService,
            IOptionsMonitor<HostOptions> hostOptions,
            ISignInManager signInManager,
            IInvitationService invitationService,
            ILogger<RegisterModel> logger,
            IEntityStore<OrganisationModel> orgStore,
            IEmailSender emailSender)
        {
            this.userService = userService;
            this.hostOptions = hostOptions;
            this.signInManager = signInManager;
            this.invitationService = invitationService;
            this.logger = logger;
            this.orgStore = orgStore;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        [BindProperty]
        [IsTrue(ErrorMessage = "You must accept the service agreement.")]
        public bool AcceptServiceAgreement { get; set; }
        // public OrganisationModel Organisation { get; private set; }
        public string ReturnUrl { get; set; }

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

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string email = null)
        {
            ReturnUrl = returnUrl;

            if (email != null)
            {
                var invitation = await invitationService.GetInvitationByEmailAsync(email);
                // this.Organisation = invitation?.TargetOrganisation;
                Input.Email = email;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    About = Input.About,
                    FullName = Input.FullName
                };

                var invitation = await invitationService.GetInvitationByEmailAsync(Input.Email);

                var result = await userService.CreateAsync(user, invitation, Input.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");
                    await SendConfirmationEmailAsync(result.Entity);

                    await signInManager.SignInAsync(result.Entity, isPersistent: false);

                    if (string.IsNullOrEmpty(invitation?.TargetOrganisationId))
                    {
                        return RedirectToPage("./Create");
                    }
                    else
                    {
                        return RedirectToPage("/Join", new { area = "organisations", orgId = invitation?.TargetOrganisationId });
                    }
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

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var code = await userService.UserManager.GenerateEmailConfirmationTokenAsync(user);

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

            await emailSender.SendEmailAsync(Input.Email, "Please confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
