using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Pages.Account.Manage
{
    public class ResendConfirmationEmailPageModel : PageModel
    {
        private readonly IOptionsMonitor<HostOptions> hostOptions;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public ResendConfirmationEmailPageModel(IOptionsMonitor<HostOptions> hostOptions,
                                                IWebHostEnvironment webHost,
                                                IEmailSender emailSender,
                                                UserManager<ApplicationUser> userManager)
        {
            this.hostOptions = hostOptions;
            this.WebHost = webHost;
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        public IWebHostEnvironment WebHost { get; private set; }
        public bool Success { get; private set; }
        public ApplicationUser? AppUser { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            await LoadProperties();
            return Page();
        }

        private async Task LoadProperties()
        {
            this.AppUser = await userManager.GetUserAsync(User);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadProperties();

            if (AppUser == null)
            {
                this.Success = false;
                return Page();
            }

            if (WebHost.IsDevelopment())
            {
                // just set the email thing to true
                AppUser.EmailConfirmed = true;
                var res = await userManager.UpdateAsync(AppUser);
            }
            else if (AppUser?.Email == null)
            {
                return BadRequest("User has no email");
            }
            else
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(AppUser);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = AppUser?.Id, code = code },
                    protocol: Request.Scheme);

                // this line is broken
                // await emailSender.SendEmailAsync(new ConfirmEmailEmail(AppUser, hostOptions.CurrentValue, code));
                await emailSender.SendEmailAsync(AppUser!.Email, "Please confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }

            this.Success = true;
            return Page();
        }
    }
}