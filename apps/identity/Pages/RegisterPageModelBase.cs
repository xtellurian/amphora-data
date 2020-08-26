using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Emails;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Pages
{
    public abstract class RegisterPageModelBase : PageModelBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<RegisterPageModelBase> logger;
        private readonly IEmailSender emailSender;

        public RegisterPageModelBase(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterPageModelBase> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        public string? ReturnUrl { get; set; }

        protected async Task<IActionResult> HandleRegistration(string? returnUrl, ApplicationUser user, string password)
        {
            this.ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                logger.LogInformation("User created a new account with password.");
                await signInManager.SignInAsync(user, isPersistent: false);
                await SendEmailsAsync(user);
                return this.LoadingPage("/Redirect", this.ReturnUrl);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }
        }

        private async Task SendEmailsAsync(ApplicationUser user)
        {
            try
            {
                await SendEmailConfirmationAsync(user);
                await SendWelcomeEmailAsync(user);
            }
            catch (System.Exception ex)
            {
                logger.LogCritical("Failed to Send email", ex);
            }

            // await emailSender.SendEmailAsync(user.Email, "Please confirm your email",
            //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            var templateData = ConfirmEmailEmail.TemplateData(user, HtmlEncoder.Default.Encode(callbackUrl));
            var content = await emailSender.Generator.ContentFromMarkdownTemplateAsync(ConfirmEmailEmail.TemplateName, templateData);
            var email = new ConfirmEmailEmail(user.Email, user.UserName, content);
            var result = await emailSender.SendEmailAsync(email);
            if (!result)
            {
                logger.LogCritical($"Failed to send email confirmation to User({user.Id})");
            }
        }

        private async Task SendWelcomeEmailAsync(ApplicationUser user)
        {
            var templateData = WelcomeEmail.TemplateData(user);
            var content = await emailSender.Generator.ContentFromMarkdownTemplateAsync(WelcomeEmail.TemplateName, templateData);
            var email = new WelcomeEmail(user.Email, user.UserName, content);
            var result = await emailSender.SendEmailAsync(email);
            if (!result)
            {
                logger.LogCritical($"Failed to send email welcome to User({user.Id})");
            }
        }
    }
}
